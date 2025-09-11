namespace DogInfoTracker {
    internal static class Validations {
        public static void ValidateUserNumChoice(out int userNumChoice) {
            int userNumInputChoice = 0;
            bool userInputChoiceParsed = false;


            while (!userInputChoiceParsed) {
                string userInputChoice = Console.ReadLine();
                userInputChoiceParsed = Int32.TryParse(userInputChoice, out userNumInputChoice);
                if (!userInputChoiceParsed) {
                    Console.WriteLine("Zadej validní číslo");
                }
            }

            userNumChoice = userNumInputChoice;
        }

        public static void ValidateOption(List<int> optionsList, out int userChoiceOption) {
            bool isValidOption = false;
            int userNumInputChoice = 0;

            while (!isValidOption) {
                Validations.ValidateUserNumChoice(out userNumInputChoice);

                if (optionsList.Contains(userNumInputChoice)) {
                    isValidOption = true;
                }
            }
            userChoiceOption = userNumInputChoice;
        }

        public static bool ValidateYear(int year) {
            return (year > 9999 || year < 1900) ? false : true;
        }

        public static bool ValidateMonth(int month) {
            return (month > 12 || month < 1) ? false : true;
        }

        public static bool ValidateDay(int day) {
            return (day > 31 || day < 1) ? false : true;
        }

        public static bool ValidateDayInMonth(int day, int month) {
            switch (month) {
                case 2:
                    if (day > 29) {
                        return false;
                    }
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    if (day > 30) {
                        return false;
                    }
                    break;
            }

            return true;
        }

        public static bool ValidateHour(int hour) {
            return (hour > 24 || hour < 0) ? false : true;
        }

        public static bool ValidateMinute(int minute) {
            return (minute > 60 || minute < 0) ? false : true;
        }
    }
}
