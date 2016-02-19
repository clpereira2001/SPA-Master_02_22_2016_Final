using System;

namespace Vauction.Models
{
  [Serializable]
  partial class ConsignorsDd : IConsignors
  {
    public long ID { get; set; }
    public string Name { get; set; }
  }
}
