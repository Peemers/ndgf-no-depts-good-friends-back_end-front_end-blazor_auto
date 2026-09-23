namespace ndgf.Web.Helpers;

public class UserColorHelper
{
  private static readonly string[] Colors =
  {
    "from-[#8A6220] to-[#6E5219]",
    "from-[#98492C] to-[#7A3A23]",
    "from-[#46392F] to-[#352A23]",
    "from-[#6A4A70] to-[#553A5A]",
    "from-[#4A5C86] to-[#3A4A6E]",
    "from-[#6E6B2A] to-[#575420]"
  };

  public static string GetColorForUser(Guid userId)
  {
    var index = Math.Abs(userId.GetHashCode()) % Colors.Length;
    return Colors[index];
  }
}