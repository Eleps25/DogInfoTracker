using Spectre.Console;

namespace DogInfoTracker {
    internal class Program {
        static void Main(string[] args) {
            DatabaseOperations.CheckTableExist();
            Console.WriteLine("Dog Activity Tracker");
            ShowMainMenu();
        }

        private static void ShowMainMenu() {
            Console.WriteLine("Hlavní menu");
            Console.WriteLine("1 - Zobrazit všechny záznamy");
            Console.WriteLine("2 - Přidat záznam");
            Console.WriteLine("3 - Aktualizovat záznam");
            Console.WriteLine("4 - Smazat záznam");
            Console.WriteLine("0 - Ukončit aplikaci");
            CallUserMainMenuChoice();
        }

        private static void CallUserMainMenuChoice() {
            int userChoice = Int32.Parse(Console.ReadLine());

            switch (userChoice) {
                case 1:
                    ShowAllRecords();
                    ShowMainMenu();
                    break;
                case 2:
                    AddRecord();
                    break;
                case 3:
                    UpdateRecord();
                    break;
                case 4:
                    DeleteRecord();
                    break;
                case 0:
                    break;
            }
        }

        private static void ShowAllRecords() {
            List<ActivityInformation> activityInformations = DatabaseOperations.GetActivityInformations();
            var table = new Table();

            table.AddColumn(new TableColumn("Id"));
            table.AddColumn(new TableColumn("Datum"));
            table.AddColumn(new TableColumn("Typ"));
            table.AddColumn(new TableColumn("Cena"));
            table.AddColumn(new TableColumn("Popis"));
            table.AddColumn(new TableColumn("Váha"));

            foreach (var activity in activityInformations) {
                table.AddRow($"{activity.Id}", $"{activity.Date}", $"{activity.ActivityType}", $"{activity.Cost}", $"{activity.Description}", $"{activity.Weight}");
            }

            AnsiConsole.Write(table);
        }

        private static void AddRecord() {
            throw new NotImplementedException();
        }

        private static void UpdateRecord() {
            throw new NotImplementedException();
        }

        private static void DeleteRecord() {
            throw new NotImplementedException();
        }
    }
}
