using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace WebDownloadr.Infrastructure.Data.Config;
// https://stevedunn.github.io/Vogen/overview.html
// Working with IDs: https://stevedunn.github.io/Vogen/working-with-ids.html
internal class VogenSequentialGuidValueGenerator<TId> : ValueGenerator<TId>
  where TId : IVogen<TId, Guid>
{

  public override TId Next(EntityEntry entry)
    => TId.From(NewSequential());

  public override bool GeneratesTemporaryValues => false;

  // Sequential GUID logic, static for all uses
  private static long _counter = DateTime.UtcNow.Ticks;

  private static Guid NewSequential()
  {
    var guidBytes = Guid.NewGuid().ToByteArray();

    var counterBytes = BitConverter.GetBytes(
      Interlocked.Increment(ref _counter));

    if (!BitConverter.IsLittleEndian)
    {
      Array.Reverse(counterBytes);
    }

    guidBytes[08] = counterBytes[1];
    guidBytes[09] = counterBytes[0];
    guidBytes[10] = counterBytes[7];
    guidBytes[11] = counterBytes[6];
    guidBytes[12] = counterBytes[5];
    guidBytes[13] = counterBytes[4];
    guidBytes[14] = counterBytes[3];
    guidBytes[15] = counterBytes[2];

    return new Guid(guidBytes);
  }
}
