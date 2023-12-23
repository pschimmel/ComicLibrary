using System.Globalization;

namespace ComicLibrary.Model.Entities
{
  /// <summary>
  /// Grade of the comic. 
  /// See also https://artfulinvestments.com/comic-book-grading/
  /// </summary>
  public class Grade : IComparable, IComparable<Grade>
  {
    private Grade(double number, string name, string description, bool isMain = false)
    {
      Number = number;
      Name = name;
      Description = description;
      IsMainGrade = isMain;
    }

    public double Number { get; }

    public string Name { get; }

    public string Description { get; }

    public bool IsMainGrade { get; }

    public override bool Equals(object obj)
    {
      return obj is Grade other && Number.Equals(other.Number);
    }

    public override int GetHashCode()
    {
      return Number.GetHashCode();
    }

    public override string ToString()
    {
      return Number > 0 ? $"{Number.ToString("0.0", CultureInfo.InvariantCulture)} - {Name}" : Name;
    }

    public int CompareTo(Grade other)
    {
      return other == null ? 1 : Number.CompareTo(other.Number);
    }

    public int CompareTo(object obj)
    {
      return obj is not Grade other ? 1 : CompareTo(other);
    }

    public static Grade[] Grades { get; } = [Unrated, GemMint, Mint, NearMintMint, NearMintPlus, NearMint, NearMintMinus, VeryFineNearMint, VeryFinePlus, VeryFine, VeryFineMinus, FineVeryFine, FinePlus, Fine, FineMinus, VeryGoodFine, VeryGoodPlus, VeryGood, VeryGoodMinus, GoodVeryGood, GoodPlus, Good, GoodMinus, FairGood, Fair, Poor];

    public static Grade Unrated => new(-1.0, "(Unrated)", Properties.Resources.UnratedDescription);

    public static Grade GemMint => new(10.0, "Gem Mint", Properties.Resources.GemMintDescription);

    public static Grade Mint => new(9.9, "Mint", Properties.Resources.MintDescription);

    public static Grade NearMintMint => new(9.8, "Near Mint/Mint", Properties.Resources.NearMintMintDescription);

    public static Grade NearMintPlus => new(9.6, "Near Mint+", Properties.Resources.NearMintPlusDescription);

    public static Grade NearMint => new(9.4, "Near Mint", Properties.Resources.NearMintDescription, true);

    public static Grade NearMintMinus => new(9.2, "Near Mint-", Properties.Resources.NearMintMinusDescription);

    public static Grade VeryFineNearMint => new(9.0, "Very Fine/Near Mint", Properties.Resources.VeryFineNearMintDescription);

    public static Grade VeryFinePlus => new(8.5, "Very Fine+", Properties.Resources.VeryFinePlusDescription);

    public static Grade VeryFine => new(8.0, "Very Fine", Properties.Resources.VeryFineDescription, true);

    public static Grade VeryFineMinus => new(7.5, "Very Fine-", Properties.Resources.VeryFineMinusDescription);

    public static Grade FineVeryFine => new(7.0, "Fine/Very Fine", Properties.Resources.FineVeryFineDescription);

    public static Grade FinePlus => new(6.5, "Fine+", Properties.Resources.FinePlusDescription);

    public static Grade Fine => new(6.0, "Fine", Properties.Resources.FineDescription, true);

    public static Grade FineMinus => new(5.5, "Fine-", Properties.Resources.FineMinusDescription);

    public static Grade VeryGoodFine => new(5.0, "Very Good/Fine", Properties.Resources.VeryGoodFineDescription);

    public static Grade VeryGoodPlus => new(4.5, "Very Good+", Properties.Resources.VeryGoodPlusDescription);

    public static Grade VeryGood => new(4.0, "Very Good", Properties.Resources.VeryGoodDescription, true);

    public static Grade VeryGoodMinus => new(3.5, "Very Good-", Properties.Resources.VeryGoodMinusDescription);

    public static Grade GoodVeryGood => new(3.0, "Good/Very Good", Properties.Resources.GoodVeryGoodDescription);

    public static Grade GoodPlus => new(2.5, "Good+", Properties.Resources.GoodPlusDescription);

    public static Grade Good => new(2.0, "Good", Properties.Resources.GoodDescription, true);

    public static Grade GoodMinus => new(1.8, "Good-", Properties.Resources.GoodMinusDescription);

    public static Grade FairGood => new(1.5, "Fair/Good", Properties.Resources.FairGoodDescription);

    public static Grade Fair => new(1.0, "Fair", Properties.Resources.FairDescription, true);

    public static Grade Poor => new(0.5, "Poor", Properties.Resources.PoorDescription, true);
  }
}