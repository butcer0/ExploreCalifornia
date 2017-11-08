using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExploreCalifornia.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExploreCalifornia.ViewComponents
{
    [ViewComponent]
    public class MonthlySpecialsViewComponent : ViewComponent
    {
        #region Depricated - Use _specials as db context
        //private readonly SpecialsDataContext _specials;
        #endregion
        private readonly SpecialsDataContext _db;
        //Erik - 10/6/2017 Inject SpecialsDataContext
        public MonthlySpecialsViewComponent(SpecialsDataContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {

            var currentSpecials = _db.GetMonthlySpecials();


            return View(currentSpecials);
        }

    }
}
