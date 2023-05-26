using Telegram.Bot;
using System.Data;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using System.Text;
using System.Data.SqlClient;
using TGbot;






namespace Telegram_Bot
{
    class Program
    { 
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("5888855766:AAHZ_qQ_gbCx_hphDWzTnn29D__6nua49OA");
            client.StartReceiving(UpdateTG, ErrorTG);
            Console.ReadLine();
        }

        static string connectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Finance;Data Source=DESKTOP-3E6ME6N";
        static string procedureNameTGBot = "TGBot";


        #region объект данных
        public class ItemsItem 
        {
            public string name { get; set; } //название товара  *
            public int price { get; set; } //цена на штуку в формате 5000 при цене 50.00 руб.   *
            public int sum { get; set; } //сумма, если пробивка стакается   *
            public float quantity { get; set; } //количество, с плавающей точкой 1.0    *
            public int paymentType { get; set; } //тип оплаты
            public int productType { get; set; } //тип продукта, хз зачем, он одинаковый везде
            public int nds { get; set; } //ндс, пока везде 6%
        }

        public class FFD
        {
            public string? Userid { get; set; }
            public string user { get; set; } //фио держателя ИП
            public string userInn { get; set; }  //ИНН
            public int requestNumber { get; set; } 
            public int shiftNumber { get; set; }
            public int operationType { get; set; } //возможно приход расход, но явно будет всегда приход как id = 1
            public int totalSum { get; set; } //общая сумма, видимо учет карты и нала одновременно  
            public int cashTotalSum { get; set; } //оплата налом
            public int ecashTotalSum { get; set; } //сумма оплаты картой в формате 5000 при цене 50.00 руб.
            public string kktRegId { get; set; }  //РН ККТ
            public string fiscalDriveNumber { get; set; } //ФН
            public int fiscalDocumentNumber { get; set; } //ФД
            public long fiscalSign { get; set; } //ФП
            public IEnumerable<ItemsItem> items { get; set; }
            public int nds18 { get; set; } //без ндс
            public int code { get; set; }
            public int fiscalDocumentFormatVer { get; set; }
            public string machineNumber { get; set; }
            public string retailPlace { get; set; } //место покупки
            public string buyerPhoneOrAddress { get; set; }
            public int prepaidSum { get; set; } //предоплата
            public int creditSum { get; set; } //сумма кредита?  = 0
            public int provisionSum { get; set; } //сумма резерва? = 0
            public int internetSign { get; set; }
            public string sellerAddress { get; set; }
            public int dateTime { get; set; } //временная метка от 1ого января 1970 в формате 1670320080, и кажись немного врет, отталкивается не от локального времени
            public int taxationType { get; set; } 
            public string localDateTime { get; set; } //дата совершения операции формата 2022-12-06T12:48

        }
        #endregion
        
        //Обработчик ошибок
        private static Task ErrorTG(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        
        async static Task UpdateTG(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            FFD obj = new FFD();
            var message = update.Message;

            if (message.Text != null)
            {
                if (message.Text.Contains("Иди нахуй"))
                {
                    FinanceContext context = new FinanceContext();
                    foreach (Operation operation in context.Operations)
                        Console.WriteLine(operation.UserInn);          
                   

                    Console.WriteLine($"{message.Chat.FirstName ?? "анон"}    |    {message.Text}");
                    await botClient.SendTextMessageAsync(message.Chat.Id, "");
                    return;
                }

                if (message.Text.Contains("Шаблон"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Загрузить\nКарта\nМагазин\nТовар, количесиво, сумма\nТовар, количесиво, сумма");
                    return;
                }

                if (message.Text.Contains("Загрузить"))
                {
                    string[] arr = message.Text.Split('\n');

                    DataTable table = new DataTable();
                    table.Columns.Add("name");
                    table.Columns.Add("price");
                    table.Columns.Add("sum");
                    table.Columns.Add("quantity");
                    table.Columns.Add("paymentType");
                    table.Columns.Add("productType");
                    table.Columns.Add("nds");
                    string[] arr2 = null;
                    int sum = 0;
                    for (int i = 3; i < arr.Length; i++)
                    {
                        arr2 = arr[i].Split(", ");
                        DataRow row = table.NewRow();
                        row["name"] = arr2[0];
                        row["price"] = Convert.ToInt32(arr2[2]) / Convert.ToInt32(arr2[1]) * 100;
                        row["sum"] = Convert.ToInt32(arr2[2]) * 100;
                        row["quantity"] = arr2[1];
                        table.Rows.Add(row);
                        sum += Convert.ToInt32(arr2[2]);
                    }

                    sum *= 100;
                    if (arr[1] == "Карта")
                        obj.ecashTotalSum = sum;
                    else
                        obj.cashTotalSum = sum;

                    obj.totalSum = sum;

                    obj.retailPlace = arr[2];

                    AddOperation(table);

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Данные загружены");
                    return;
                }
            }

            if (message.Photo != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "!!!");
            }

            

            if (message.Document != null)
            {
                var fileId = update.Message.Document.FileId;
                var fileInfo = await botClient.GetFileAsync(fileId);
                var filePath = fileInfo.FilePath;

                string destinationFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{message.Document.FileName}"; //путь сохранения файла
                await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                await botClient.DownloadFileAsync(filePath, fileStream);
                fileStream.Close(); //нужно закрывать, иначе он будет занят

                
                using (FileStream fstream = System.IO.File.OpenRead(destinationFilePath))
                {
                    // выделяем массив для считывания данных из файла
                    byte[] buffer = new byte[fstream.Length];
                    // считываем данные
                    await fstream.ReadAsync(buffer, 0, buffer.Length);
                    // декодируем байты в строку
                    string textFromFile = Encoding.Default.GetString(buffer);
                    // конвертация json в объект
                    obj = JsonConvert.DeserializeObject<FFD>(textFromFile);
                }

                DataTable table = new DataTable();
                table.Columns.Add("name");
                table.Columns.Add("price");
                table.Columns.Add("sum");
                table.Columns.Add("quantity");
                table.Columns.Add("paymentType");
                table.Columns.Add("productType");
                table.Columns.Add("nds");

                foreach (var str in obj.items)
                {
                    DataRow row = table.NewRow();
                    row["name"] = str.name.Trim();
                    row["price"] = str.price;
                    row["sum"] = str.sum;
                    row["quantity"] = str.quantity;
                    row["paymentType"] = str.paymentType;
                    row["productType"] = str.productType;
                    row["nds"] = str.nds;
                    table.Rows.Add(row);
                }

                AddOperation(table);

                //отправка сообщения в чат
                await botClient.SendTextMessageAsync(message.Chat.Id, "Чек загружен.");            
            }
           
            void AddOperation(DataTable table)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(procedureNameTGBot, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    //таблица Operation
                    SqlParameter codeParam  = new SqlParameter  { ParameterName = "Code", Value = obj.code };     command.Parameters.Add(codeParam);
                    SqlParameter nds18Param = new SqlParameter  { ParameterName = "Nds18", Value = obj.nds18 };   command.Parameters.Add(nds18Param);
                    SqlParameter totalSumParam = new SqlParameter { ParameterName = "TotalSum", Value = obj.totalSum }; command.Parameters.Add(totalSumParam);
                    SqlParameter kktRegIdParam = new SqlParameter { ParameterName = "KktRegId", Value = obj.kktRegId }; command.Parameters.Add(kktRegIdParam);
                    SqlParameter dateTimeParam = new SqlParameter { ParameterName = "DateTime", Value = obj.dateTime }; command.Parameters.Add(dateTimeParam);
                    SqlParameter creditSumParam   = new SqlParameter { ParameterName = "CreditSum", Value = obj.creditSum };     command.Parameters.Add(creditSumParam);
                    SqlParameter fiscalSignParam  = new SqlParameter { ParameterName = "FiscakSign", Value = obj.fiscalSign };   command.Parameters.Add(fiscalSignParam);
                    SqlParameter prepaidSumParam  = new SqlParameter { ParameterName = "PrepaidSum", Value = obj.prepaidSum };   command.Parameters.Add(prepaidSumParam);
                    SqlParameter shiftNumberParam = new SqlParameter { ParameterName = "ShiftNumber", Value = obj.shiftNumber }; command.Parameters.Add(shiftNumberParam);
                    SqlParameter cashTotalSumParam = new SqlParameter { ParameterName = "CashTotalSum", Value = obj.cashTotalSum }; command.Parameters.Add(cashTotalSumParam);
                    SqlParameter provisionSumParam = new SqlParameter { ParameterName = "ProvisionSum", Value = obj.provisionSum }; command.Parameters.Add(provisionSumParam);
                    SqlParameter internetSignParam = new SqlParameter { ParameterName = "InternetSign", Value = obj.internetSign }; command.Parameters.Add(internetSignParam);
                    SqlParameter taxationTypeParam = new SqlParameter { ParameterName = "TaxationType", Value = obj.taxationType }; command.Parameters.Add(taxationTypeParam);
                    SqlParameter ecashTotalSumParam = new SqlParameter { ParameterName = "ECashTotalSum", Value = obj.ecashTotalSum }; command.Parameters.Add(ecashTotalSumParam);
                    SqlParameter requestNumberParam = new SqlParameter { ParameterName = "RequestNumber", Value = obj.requestNumber }; command.Parameters.Add(requestNumberParam);
                    SqlParameter machineNumberParam = new SqlParameter { ParameterName = "MachineNumber", Value = obj.machineNumber }; command.Parameters.Add(machineNumberParam);
                    SqlParameter sellerAddressParam = new SqlParameter { ParameterName = "SellerAddress", Value = obj.sellerAddress }; command.Parameters.Add(sellerAddressParam);
                    SqlParameter operationTypeParam = new SqlParameter { ParameterName = "OperationType", Value = obj.operationType }; command.Parameters.Add(operationTypeParam);
                    SqlParameter fiscalDriveNumberParam       = new SqlParameter { ParameterName = "FiscalDriveNumber", Value = obj.fiscalDriveNumber };       command.Parameters.Add(fiscalDriveNumberParam);
                    SqlParameter buyerPhoneOrAddressParam     = new SqlParameter { ParameterName = "BuyerPhoneOrAddress", Value = obj.buyerPhoneOrAddress };   command.Parameters.Add(buyerPhoneOrAddressParam);                
                    SqlParameter fiscalDocumentNumberParam    = new SqlParameter { ParameterName = "FiscalDocumentNumber", Value = obj.fiscalDocumentNumber };       command.Parameters.Add(fiscalDocumentNumberParam);      
                    SqlParameter fiscalDocumentFormatVerParam = new SqlParameter { ParameterName = "FiscalDocumentFormatVer", Value = obj.fiscalDocumentFormatVer }; command.Parameters.Add(fiscalDocumentFormatVerParam);

                    //таблица RetailPlace
                    SqlParameter userParam = new SqlParameter { ParameterName = "ShopUser", Value = obj.user }; command.Parameters.Add(userParam);
                    SqlParameter userInnParam = new SqlParameter { ParameterName = "ShopInn", Value = obj.userInn }; command.Parameters.Add(userInnParam);
                    SqlParameter retailPlaceParam = new SqlParameter { ParameterName = "ShopName", Value = obj.retailPlace.Trim() }; command.Parameters.Add(retailPlaceParam);

                    //таблица ItemsTable
                    SqlParameter itemsTable = new SqlParameter { ParameterName = "ItemsTable", Value = table, SqlDbType = SqlDbType.Structured, TypeName = "dbo.typeTableItemsBuy" }; command.Parameters.Add(itemsTable);

                    var result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}