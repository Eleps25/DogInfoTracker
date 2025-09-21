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
                    Console.Clear();
                    ShowAllRecords();
                    ShowMainMenu();
                    break;
                case 2:
                    Console.Clear();
                    AddRecord();
                    ShowMainMenu();
                    break;
                case 3:
                    Console.Clear();
                    UpdateRecord();
                    ShowMainMenu();
                    break;
                case 4:
                    Console.Clear();
                    DeleteRecord();
                    ShowMainMenu();
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
                table.AddRow($"{activity.Id}", $"{activity.Date}", $"{EnumHelper.GetEnumDescription(activity.ActivityType)}", $"{activity.Cost}", $"{activity.Description}", $"{activity.Weight}");
            }

            AnsiConsole.Write(table);
        }

        private static void AddRecord() {
            int[] dateAsParts = new int[3];
            DateTime date = GetUserChoiceDate("Zadej informace o datumu", out dateAsParts);
            int year = dateAsParts[0];
            int month = dateAsParts[1];
            int day = dateAsParts[2];
            int activityType = GetUserChoiceActivityType();
            int cost = 0;
            string description = "";
            float weight = 0f;

            if (activityType != (int)ActivityType.Weight) {
                cost = GetCost();
            } else {
                weight = GetWeight();
            }
            description = GetDescription();

            DatabaseOperations.InsertRecordToDatabase(date, year, month, day, activityType, cost, description, weight);
        }

        private static void UpdateRecord() {
            Console.Clear();
            bool isUpdating = true;
            ShowAllRecords();
            List<int> optionsList = DatabaseOperations.GetActivityIds();
            Console.WriteLine("Vyber záznam k aktualizaci");
            int userRecordIdChoice;
            Validations.ValidateOption(optionsList, out userRecordIdChoice, "Vyber platný záznam.");

            ActivityInformation activityInformation = DatabaseOperations.GetActivityInformationRecord(userRecordIdChoice);
            Console.WriteLine($"Zvolený záznam: {activityInformation.Id} - {activityInformation.Date} _ {EnumHelper.GetEnumDescription(activityInformation.ActivityType)} - Cena: {activityInformation.Cost} _ Popis: {activityInformation.Description} _ Váha: {activityInformation.Weight}");

            DateTime date = activityInformation.Date;
            int[] dateAsParts = new int[3];

            while (isUpdating) {
                int userChoice = GetUserUpdateChoice();

                switch (userChoice) {
                    case 1:
                        activityInformation.Date = GetUserChoiceDate("Zadej informace o datumu", out dateAsParts);
                        activityInformation.Year = dateAsParts[0];
                        activityInformation.Month = dateAsParts[1];
                        activityInformation.Day = dateAsParts[2];
                        break;
                    case 2:
                        activityInformation.ActivityType = (ActivityType)GetUserChoiceActivityType();
                        break;
                    case 3:
                        activityInformation.Cost = GetCost();
                        break;
                    case 4:
                        activityInformation.Description = GetDescription();
                        break;
                    case 5:
                        activityInformation.Weight = GetWeight();
                        break;
                    case 6:
                        Console.WriteLine($"Upravený záznam: {activityInformation.Date} - typ: {activityInformation.ActivityType}, cena: {activityInformation.Cost}, popis: {activityInformation.Description}, váha: {activityInformation.Weight}");
                        AnsiConsole.Markup("Chceš takto upravený záznam uložit? ([green]1 - ano[/], [red]2 - ne[/])");
                        userChoice = Int32.Parse(Console.ReadLine());
                        if (userChoice == 1) {
                            DatabaseOperations.UpdateRecord(activityInformation.Id, activityInformation.Date, activityInformation.Year, activityInformation.Month, activityInformation.Day, (int)activityInformation.ActivityType, activityInformation.Cost, activityInformation.Description, activityInformation.Weight);
                            isUpdating = false;
                            Console.Clear();
                            AnsiConsole.Markup("[green]Záznam byl změněn.\n[/]");
                        } else if (userChoice == 2) {
                            userChoice = GetUserUpdateChoice();
                        }
                        break;
                    case 7:
                        isUpdating = false;
                        UpdateRecord();
                        break;
                    case 8:
                        isUpdating = false;
                        break;
                }
            }
        }

        private static void DeleteRecord() {
            ShowAllRecords();
            Console.WriteLine("Vyber záznam k vymazání nebo stiskni 0 pro návrat do hlavního menu.");
            List<int> optionsList = DatabaseOperations.GetActivityIds();
            optionsList.Add(0);
            int userRecordIdChoice;
            Validations.ValidateOption(optionsList, out userRecordIdChoice, "Vyber platný záznam.");

            if (userRecordIdChoice != 0) {
                ActivityInformation activityInformation = DatabaseOperations.GetActivityInformationRecord(userRecordIdChoice);
                Console.WriteLine($"Zvolený záznam: {activityInformation.Id} - {activityInformation.Date} _ {EnumHelper.GetEnumDescription(activityInformation.ActivityType)} - Cena: {activityInformation.Cost} _ Popis: {activityInformation.Description} _ Váha: {activityInformation.Weight}");
                AnsiConsole.Markup("Chceš tento záznam smazat? ([green]1 - ano[/], [red]2 - ne[/]):\n");
                int userChoice = Int32.Parse(Console.ReadLine());
                Console.Clear();
                if (userChoice == 1) {
                    DatabaseOperations.DeleteRecord(userRecordIdChoice);
                    AnsiConsole.Markup("[green]Záznam byl odstraněn.\n[/]");
                } else if (userChoice == 2) {
                    DeleteRecord();
                }
            } else {
                Console.Clear();
            }
        }

        private static DateTime GetUserChoiceDate(string message, out int[] datePartsOut) {
            int[] dateParts = new int[3];
            bool isValidInput = false;
            int year = 0;
            int month = 0;
            int day = 0;

            Console.WriteLine(message);

            Console.WriteLine("Zadej rok (formát: RRRR):");
            while (!isValidInput) {
                year = Int32.Parse(ReadLineWithLimit(4));
                if (Validations.ValidateYear(year)) {
                    isValidInput = true;
                } else {
                    Console.WriteLine("Zadej platný rok (formát: RRRR):");
                }
            }
            isValidInput = false;

            Console.WriteLine("Zadej měsíc (formát: MM):");
            while (!isValidInput) {
                month = Int32.Parse(ReadLineWithLimit(2));
                if (Validations.ValidateMonth(month)) {
                    isValidInput = true;
                } else {
                    Console.WriteLine("Zadej platný měsíc (formát: MM):");
                }
            }
            isValidInput = false;

            Console.WriteLine("Zadej den (formát: DD):");
            while (!isValidInput) {
                day = Int32.Parse(ReadLineWithLimit(2));
                if (Validations.ValidateDay(day)) {
                    if (Validations.ValidateDayInMonth(day, month)) {
                        isValidInput = true;
                    } else {
                        Console.WriteLine("Zadej platný den (formát: DD):");
                    }
                } else {
                    Console.WriteLine("Zadej platný den (formát: DD):");
                }
            }
            isValidInput = false;

            isValidInput = false;
            dateParts[0] = year;
            dateParts[1] = month;
            dateParts[2] = day;
            datePartsOut = dateParts;

            return new DateTime(year, month, day);
        }

        private static int GetUserChoiceActivityType() {
            bool isValidInput = false;
            int activityTypeNumber = 0;

            while (!isValidInput) {
                Console.WriteLine("Vyber typ:");

                foreach (ActivityType activityType in Enum.GetValues(typeof(ActivityType))) {
                    string desc = EnumHelper.GetEnumDescription(activityType);
                    Console.WriteLine($"{(int)activityType} - {desc}");
                }

                Console.Write("\nZadejte číslo vybraného typu: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out activityTypeNumber) && Enum.IsDefined(typeof(ActivityType), activityTypeNumber)) {
                    isValidInput = true;
                }
            }

            return activityTypeNumber;
        }

        public static int GetUserUpdateChoice() {
            /*DateTime date = activityInformation.Date;
            int[] dateAsParts = new int[3];*/

            Console.WriteLine("Co chceš udělat?");
            Console.WriteLine("1 - Aktualizovat datum");
            Console.WriteLine("2 - Aktualizovat typ");
            Console.WriteLine("3 - Aktualizovat cenu");
            Console.WriteLine("4 - Aktualizovat popis");
            Console.WriteLine("5 - Aktualizovat váhu");
            Console.WriteLine("6 - Povtrdit změny a upravit záznam");
            Console.WriteLine("7 - Vybrat jiný záznam");
            Console.WriteLine("8 - Vrátit se do hlavního menu");

            //int userChoice = Int32.Parse(Console.ReadLine());
            return Int32.Parse(Console.ReadLine());

            /*switch (userChoice) {
                case 1:
                    activityInformation.Date = GetUserChoiceDate("Zadej informace o datumu", out dateAsParts);
                    activityInformation.Year = dateAsParts[0];
                    activityInformation.Month = dateAsParts[1];
                    activityInformation.Day = dateAsParts[2];
                    GetUserUpdateChoice(activityInformation);
                    break;
                case 2:
                    activityInformation.ActivityType = (ActivityType)GetUserChoiceActivityType();
                    break;
                case 3:
                    activityInformation.Cost = GetCost();
                    break;
                case 4:
                    activityInformation.Description = GetDescription();
                    break;
                case 5:
                    activityInformation.Weight = GetWeight();
                    break;
                case 6:
                    Console.WriteLine($"Upravený záznam: {activityInformation.Date} - typ: {activityInformation.ActivityType}, cena: {activityInformation.Cost}, popis: {activityInformation.Description}, váha: {activityInformation.Weight}");
                    Console.WriteLine("Chceš takto upravený záznam uložit? [1 - ano, 2 - ne]");
                    userChoice = Int32.Parse(Console.ReadLine());
                    if (userChoice == 1) {
                        DatabaseOperations.UpdateRecord(activityInformation.Id, activityInformation.Date, activityInformation.Year, activityInformation.Month, activityInformation.Day, activityInformation.ActivityType, activityInformation.Cost, activityInformation.Description, activityInformation.Weight);
                    } else if (userChoice == 2) {
                        GetUserUpdateChoice(activityInformation);
                    }
                    break;
                case 7:
                    UpdateRecord();
                    break;
                case 8:
                    break;
            }*/
        }

        public static int GetCost() {
            Console.WriteLine("Zadej cenu:");
            return Int32.Parse(Console.ReadLine());
        }

        public static float GetWeight() {
            Console.WriteLine("Zadej váhu:");
            return float.Parse(Console.ReadLine());
        }

        public static string GetDescription() {
            Console.WriteLine("Zadej popis:");
            return Console.ReadLine();
        }

        private static string ReadLineWithLimit(int noOfChar) {
            string input = "";

            while (true) {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter) {
                    Console.WriteLine();
                    break;
                }

                // Zpět - odstraní poslední znak
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) {
                    input = input.Substring(0, input.Length - 1);
                    Console.Write("\b \b"); // smaže znak z konzole
                }
                // Přidání znaku, pokud ještě není dosažen limit
                else if (!char.IsControl(key.KeyChar) && input.Length < noOfChar) {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
            return input;
        }
    }
}
