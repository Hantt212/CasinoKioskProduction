using CKDatabaseConnection.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace CKDatabaseConnection.DAO
{
    public class ItemDao
    {
        ITHoTram_CustomReportEntities context = null;
        public ItemDao()
        {
            context = new ITHoTram_CustomReportEntities();
        }
        public IEnumerable<CasinoKiosk_Item> ListAllPaging(int page, int pageSize)
        {
            return context.CasinoKiosk_Item.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public long Insert(CasinoKiosk_Item entity)
        {
            context.CasinoKiosk_Item.Add(entity);
            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return entity.ID;
        }

        public bool Update(CasinoKiosk_Item entity)
        {
            try
            {
                var item = context.CasinoKiosk_Item.Find(entity.ID);

                item.ItemName = entity.ItemName;
                item.ItemPoints = entity.ItemPoints;
                item.Quantity = entity.Quantity;
                item.isActive = entity.isActive;
                item.imageURL = entity.imageURL;
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public CasinoKiosk_Item ViewDetail(int id)
        {
            return context.CasinoKiosk_Item.Find(id);

        }

        public CasinoKiosk_Item GetItemByName(string itemName)
        {
            return context.CasinoKiosk_Item.SingleOrDefault(x => x.ItemName == itemName);
        }

        public CasinoKiosk_Item GetItemByID(int itemID)
        {
            return context.CasinoKiosk_Item.SingleOrDefault(x => x.ID == itemID);
        }

        
    }
}
