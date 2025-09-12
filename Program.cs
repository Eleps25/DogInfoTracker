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
                    ShowMainMenu();
                    break;
                case 3:
                    UpdateRecord();
                    ShowMainMenu();
                    break;
                case 4:
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
                Console.WriteLine("Zadej cenu:");
                cost = Int32.Parse(Console.ReadLine());
            } else {
                Console.WriteLine("Zadej váhu:");
                weight = float.Parse(Console.ReadLine());
            }
            Console.WriteLine("Zadej popis:");
            description = Console.ReadLine();

            DatabaseOperations.InsertRecordToDatabase(date, year, month, day, activityType, cost, description, weight);
        }

        private static void UpdateRecord() {
            throw new NotImplementedException();
        }

        private static void DeleteRecord() {
            DatabaseOperations.GetActivityInformations();
            Console.WriteLine("Vyber záznam k vymazání");
            int recordIdToDelete = Int32.Parse(Console.ReadLine());
            DatabaseOperations.DeleteRecord(recordIdToDelete);
        }

        private static DateTime GetUserChoiceDate(string message, out int[] datePartsOut) {
            int[] dateParts = new int[3];
            bool isValidInput = false;
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;

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

            Console.WriteLine("Zadej hodinu (formát: HH):");
            while (!isValidInput) {
                hour = Int32.Parse(ReadLineWithLimit(2));
                if (Validations.ValidateHour(hour)) {
                    isValidInput = true;
                } else {
                    Console.WriteLine("Zadej platnou hodinu (formát: HH):");
                }
            }
            isValidInput = false;

            Console.WriteLine("Zadej minuty (formát: MM):");
            while (!isValidInput) {
                minute = Int32.Parse(ReadLineWithLimit(2));
                if (Validations.ValidateMinute(minute)) {
                    isValidInput = true;
                } else {
                    Console.WriteLine("Zadej platné minuty (formát: MM):");
                }
            }
            isValidInput = false;
            dateParts[0] = year;
            dateParts[1] = month;
            dateParts[2] = day;
            datePartsOut = dateParts;

            return new DateTime(year, month, day, hour, minute, 0);
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
