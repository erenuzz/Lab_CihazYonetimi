﻿using DataAccess.Abstract;
using DataAccess.Concrete;
using DataAccess.Repository;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntityFramework
{
    public class EfCihazDal:GenericRepository<Cihazlar> , ICihazDal
    {
    }
}