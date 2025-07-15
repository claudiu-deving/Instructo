namespace Domain.Models;
/// <summary>
/// Defines the statistics for the School, will be used to store various metrics in JSON format in the database column
/// </summary>
public class Statistics
{
    public Statistics()
    {

    }

    /// <summary>
    /// The number of students that passed with this School.
    /// </summary>
    public int NumberOfStudents { get; set; }


    public static Statistics Empty => new Statistics
    {
        NumberOfStudents=0
    };
}
