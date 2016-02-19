using System;
using System.Linq;
using System.Collections.Generic;

namespace Vauction.Models
{
  public interface ICountryRepository
  {
    List<Country> GetCountryList();
    Country GetCountryByID(long ID);    
    List<State> GetStateList(long? country_id);
    State GetStateByCode(string code);
  }
}