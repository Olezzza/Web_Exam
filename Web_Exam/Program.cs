using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using MySql.Data.MySqlClient;

namespace Web_Exam
{
    class Program
    {
        /*
         * Используя возможности selenium web-driver подключиться к сайту https://habr.com/ru/ спарсить данные 
         * по лучшим публикациям за сутки\неделю\месяц\год: название, описание, рейтинг, кол-во просм-в, имя автора, дата добав-я. 
         * Данные должны автоматически собираться каждый час. Для хранения полученных данных спроектировать БД. 
         * При парсинге новых данных проверять нет ли указанной публикации с такими же характеристиками в БД. 
         * Отдельно реализовать консольную команду для печати данных по выбранной публикации в pdf-документ.
         * 
         * При наличии пагинации на странице первый раз пройти по всем страницам, далее, при последующих итерациях парсить 
         * только обновленные данные.
        */

        static bool continueProgram = true;

        static void Main()
        {
            DatabaseWork.setConnection();
            //DatabaseWork.deleteAllRows();

            HabrParser parser = new HabrParser();
            int number = 0;
            bool isFirstOpen = true, isContinue = true;
            Article articleForFind;


            do {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Начать парсинг.");
                Console.WriteLine("2. Вывести все статьи на экран.");
                Console.WriteLine("3. Получить статью.");
                Console.WriteLine("4. Завершить программу.");
                Console.WriteLine("5. Удалить записи из БД.");

                number = Int32.Parse(Console.ReadLine().ToString());

                switch (number) {
                    case 1:
                        while (isContinue)
                        {
                            if (isFirstOpen) {
                                parser.parse();
                                isFirstOpen = !isFirstOpen;
                            } else
                                parser.dailyPrser();

                            Console.WriteLine("Оставить почасовое сканирование? (n/y)");
                           
                            if (Console.ReadLine() == "n")
                                break;

                            //Thread.Sleep(86400000);
                            Thread.Sleep(600000);
                        }
                        break;
                    case 2:
                        DatabaseWork.readRows();
                        break;
                    case 3:
                        Console.WriteLine("Введие название статьи: ");
                        string title = Console.ReadLine().Trim();
                        articleForFind = DatabaseWork.findArticleInDB(title);
                        Article.printArticle(articleForFind);
                        Article.printToPDF(articleForFind);
                        break;
                    case 4:
                        continueProgram = !continueProgram;
                        break;
                    case 5:
                        DatabaseWork.deleteAllRows();
                        break;
                    default:
                        break;
                }

            } while (continueProgram);
        }



    }
}