using Microsoft.EntityFrameworkCore;

namespace HW_22_09
{
     class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                //1
                db.Database.ExecuteSqlRaw("INSERT INTO Stations (Name) VALUES ('Station1')");
                db.Database.ExecuteSqlRaw("INSERT INTO Trains (Number, Model, TravelTime, ManufacturingDate, StationId) " +
                                     "VALUES ('332', 'Model1', '04:30:00', '2022-01-01', 1)");
                //2
                var trainsMoreThan5Hours = db.Trains.FromSqlRaw(
               "SELECT * FROM Trains WHERE TravelTime > '05:00:00'").ToList();

                //3
                var stationInfo = db.Stations.FromSqlRaw(
                    "SELECT s.Id s.Name, t.Number, t.Model, t.TravelTime " +
                    "FROM Stations s JOIN Trains t ON s.Id = t.StationId").ToList();

                //4
                var stationsWithMoreThan3Trains = db.Stations.FromSqlRaw(
                    "SELECT  s.Name FROM Stations s JOIN Trains t ON s.Id = t.StationId " +
                    "GROUP BY s.Name HAVING COUNT(t.Id) > 3").ToList();

                //5
                var trainsStartingWithPell = db.Trains.FromSqlRaw(
                    "SELECT * FROM Trains WHERE Model LIKE 'Pell%'").ToList();

                //6
                var trainsOlderThan15Years = db.Trains.FromSqlRaw(
                    "SELECT * FROM Trains WHERE DATEDIFF(YEAR, ManufacturingDate, GETDATE()) > 15").ToList();

                //7
                var stationsWithTrainLessThan4Hours = db.Stations.FromSqlRaw(
                    "SELECT DISTINCT s.Name FROM Stations s " +
                    "JOIN Trains t ON s.Id = t.StationId WHERE t.TravelTime < '04:00:00'").ToList();

                //8
                var stationsWithoutTrains = db.Stations.FromSqlRaw(
                    "SELECT s.* FROM Stations s LEFT JOIN Trains t ON s.Id = t.StationId WHERE t.Id IS NULL").ToList();
            }
        }
    }
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        ICollection<Station> Stations { get; set; }

    }

    public class Train
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Model { get; set; }
        public TimeSpan TravelTime { get; set; }
        public DateOnly ManufacturingDate { get; set; }
        public int StationId { get; set; }
        public Station Station { get; set; }

    }
    public class ApplicationContext : DbContext
    {
        public DbSet<Station> Stations { get; set;}
        public DbSet<Train> Trains { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer("Server=DESKTOP-TBASQVJ;Database=testdb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
