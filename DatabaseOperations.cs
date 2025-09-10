using Dapper;
using System.Configuration;
using System.Data.SQLite;

namespace DogInfoTracker {
    internal static class DatabaseOperations {
        private static string connectionString = ConfigurationManager.AppSettings.Get("connectString");
        private static string tableName = ConfigurationManager.AppSettings.Get("tableName");
        public static void CheckTableExist() {
            using (var connection = new SQLiteConnection(connectionString)) {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                            $@"CREATE TABLE IF NOT EXISTS {tableName} (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Date TEXT,
                            Year INTEGER,
                            Month INTEGER,
                            Day INTEGER,
                            ActivityType INTEGER,
                            Cost INTEGER,
                            Description TEXT,
                            Weight REAL
                            )";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        public static void InsertRecordToDatabase(DateTime date, int year, int month, int day, int activityType = 100, int cost = 0, string description = "", float weight = 0f) {
            using (var connection = new SQLiteConnection(connectionString)) {

                var insertSql = $"INSERT INTO {tableName} (Date, Year, Month, Day, ActivityType, Cost, Description, Weight) VALUES (@Date, @Year, @Month, @Day, @ActivityType, @Cost, @Description, @Weight)";
                object[] parameters = { new { Date = date, Year = year, Mont = month, Day = day, ActivityType = activityType, Cost = cost, Description = description, Weight = weight } };
                connection.Execute(insertSql, parameters);
            }
        }

        public static List<ActivityInformation> GetActivityInformations() {
            List<ActivityInformation> activityInformations = new List<ActivityInformation>();
            activityInformations.Clear();
            using (var connection = new SQLiteConnection(connectionString)) {
                var sql = $"SELECT * FROM {tableName}";
                //object[] parameters = { new { TableName = tableName } };

                var reader = connection.ExecuteReader(sql);
                while (reader.Read()) {
                    string date = reader.GetString(reader.GetOrdinal("Date"));
                    int year = reader.GetInt32(reader.GetOrdinal("Year"));
                    int month = reader.GetInt32(reader.GetOrdinal("Month"));
                    int day = reader.GetInt32(reader.GetOrdinal("Day"));
                    int activityTypeIndex = reader.GetInt32(reader.GetOrdinal("ActivityType"));
                    int cost = reader.GetInt32(reader.GetOrdinal("Cost"));
                    string description = reader.GetString(reader.GetOrdinal("Description"));
                    float weight = reader.GetFloat(reader.GetOrdinal("Weight"));
                    ActivityInformation activityInformation = new ActivityInformation();

                    activityInformation.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                    activityInformation.Date = DateTime.Parse(date);
                    activityInformation.Year = year;
                    activityInformation.Month = month;
                    activityInformation.Day = day;
                    activityInformation.ActivityType = (ActivityType)activityTypeIndex;
                    activityInformation.Cost = cost;
                    activityInformation.Description = description;
                    activityInformation.Weight = weight;

                    activityInformations.Add(activityInformation);
                }

                return activityInformations;
            }
        }

        public static ActivityInformation GetActivityInformationRecord(int recordId) {
            List<ActivityInformation> activityInformations = GetActivityInformations();
            foreach (var activityInformation in activityInformations) {
                if (activityInformation.Id == recordId) {
                    return activityInformation;
                }
            }
            return null;
        }

        public static List<int> GetActivityIds() {
            List<int> activityIds = new List<int>();
            List<ActivityInformation> activities = GetActivityInformations();

            foreach (var activity in activities) {
                activityIds.Add(activity.Id);
            }

            return activityIds;
        }

        public static int DeleteRecord(int recordId) {
            using (var connection = new SQLiteConnection(connectionString)) {
                string deleteSql = $"DELETE FROM {tableName} WHERE Id = @ActivityId";
                object[] parameters = { new { ActivityId = recordId } };
                return connection.Execute(deleteSql, parameters);
            }
        }



        public static int UpdateRecord(int recordId, DateTime date = new DateTime(), int year = 0, int month = 0, int day = 0, int activityType = 100, int cost = 0, string description = "", float weight = 0f) {
            using (var connection = new SQLiteConnection(connectionString)) {
                //string updateSql = $"UPDATE {tableName} SET IsActiveStopwatch = @IsActiveStopwatch, Duration = @Duration";
                string updateSql = $"UPDATE {tableName} SET";
                bool updateMoreInfos = false;
                bool dateUpdated = false;
                bool activityTypeUpdated = false;
                bool costUpdated = false;
                bool descriptionUpdated = false;
                bool weightUpdated = false;

                if (year != 0) {
                    updateSql = $"{updateSql} Date = @Date, Year = @Year, Month = @Month, Day = @Day";
                    updateMoreInfos = true;
                    dateUpdated = true;
                }
                if (activityType != 100) {
                    if (updateMoreInfos) {
                        updateSql = $"{updateSql}, ActivityType = @ActivityType";
                    } else {
                        updateSql = $"{updateSql} ActivityType = @ActivityType";
                    }
                    activityTypeUpdated = true;
                    updateMoreInfos = true;
                }
                if (cost != 0) {
                    if (updateMoreInfos) {
                        updateSql = $"{updateSql}, Cost = @Cost";
                    } else {
                        updateSql = $"{updateSql} Cost = @Cost";
                    }
                    costUpdated = true;
                    updateMoreInfos = true;
                }
                if (description != "") {
                    if (updateMoreInfos) {
                        updateSql = $"{updateSql}, Description = @Description";
                    } else {
                        updateSql = $"{updateSql} Description = @Description";
                    }
                    descriptionUpdated = true;
                    updateMoreInfos = true;
                }
                if (weight != 0f) {
                    if (updateMoreInfos) {
                        updateSql = $"{updateSql}, Weight = @Weight";
                    } else {
                        updateSql = $"{updateSql} Weight = @Weight";
                    }
                    weightUpdated = true;
                    updateMoreInfos = true;
                }
                updateSql = $"{updateSql} WHERE Id = @RecordId";
                object[] parameters = { new { RecordId = recordId, Date = date, Year = year, Mont = month, Day = day, ActivityType = activityType, Cost = cost, Description = description, Weight = weight } };
                return connection.Execute(updateSql, parameters);
            }
        }
    }
}

