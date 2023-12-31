﻿using CKDatabaseConnection.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CKDatabaseConnection.DAO
{
    public class PlayerDao
    {        
        ITHoTram_CustomReportEntities context = null;
        RA_SecurityEntities RAcontext = null;

        string userNamecurr = HttpContext.Current.Session["UserName"].ToString();
        public PlayerDao()
        {
            context = new ITHoTram_CustomReportEntities();
            RAcontext = new RA_SecurityEntities();
        }
        public long Insert(CIFE_Players entity)
        {
            context.CIFE_Players.Add(entity);
            context.SaveChanges();
            return entity.ID;
        }
        public List<CIFE_Players> ListAllPlayers()
        {
            //List<CIFE_Players> listPlayers = new List<CIFE_Players>();
            return context.CIFE_Players.OrderByDescending(x => x.ID).ToList();
            //return listPlayers;
        }
        public IEnumerable<spReportCIFE_Program_Result> ListPlayersDetail(int page, int pageSize)
        {
            List<spReportCIFE_Program_Result> list = context.spReportCIFE_Program().ToList();
            return list.OrderByDescending(x => x.AccountingDate).ToPagedList(page, pageSize);
        }
        public CIFE_Players ViewDetail(int id)
        {
            return context.CIFE_Players.Find(id);
        }
        //Add 20230301 Hantt start
        public IEnumerable<SpReport_MarketingAuthorizer_Result> ListAllPagingMarketAuth()
        {
            List<SpReport_MarketingAuthorizer_Result> list = context.SpReport_MarketingAuthorizer().ToList();

            return list;
        }
        //Add 20230301 Hantt end

        public bool Update(CIFE_Players entity)
        {
            try
            {
                var player = context.CIFE_Players.Find(entity.ID);

                player.PlayerID = entity.PlayerID;          
                
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public long Delete(int id)
        {
            CIFE_Players player = new CIFE_Players() { ID = id };
            context.CIFE_Players.Attach(player);
            context.CIFE_Players.Remove(player);
            context.SaveChanges();
            return player.ID;
        }
        


        //Add fake Card Id start
        public long InsertFCardID(string fcardId, string patronId, string passportId, bool isVisitor)
        {
            //Find Patron ID
            FCardIDRefPID cardInfoByPID = RAcontext.FCardIDRefPIDs.Where(item => item.PID == patronId && item.IsActive == true).OrderByDescending(item => item.DateInserted).FirstOrDefault();

            //Find Card ID
            FCardIDRefPID cardInfoByID = RAcontext.FCardIDRefPIDs.Where(item => item.FCardID == fcardId && item.IsActive == true).OrderByDescending(item => item.DateInserted).FirstOrDefault();

            //Create new FCard
            FCardIDRefPID cardInfoNew = new FCardIDRefPID();
            long exist = 0;

            //Session timeout
            if (userNamecurr == null)
            {
                return -3;
            }


            if (cardInfoByPID != null) {
                exist = -1;
            } else {
                if (cardInfoByID != null)
                {
                    exist = -2;
                }
                else
                {
                    cardInfoNew.FCardID = fcardId;
                    cardInfoNew.PID = patronId;
                    cardInfoNew.IsActive = true;
                    cardInfoNew.PassportID = passportId;
                    cardInfoNew.DateInserted = DateTime.Now;
                    cardInfoNew.UpdatedBy = userNamecurr;
                    if (isVisitor == true)
                    {
                        cardInfoNew.Remark = "Visitor";
                    }else
                    {
                        cardInfoNew.Remark = "Member";
                    }

                    RAcontext.FCardIDRefPIDs.Add(cardInfoNew);
                    RAcontext.SaveChanges();
                    exist = cardInfoNew.ID;
                }
                
            }

            return exist;
        }

        public List<FCardIDRefPID> getFCardInfoList()
        {
            List<FCardIDRefPID> result = RAcontext.FCardIDRefPIDs.Where(item => item.IsActive == true).ToList();
            return result;
        }

        public FCardIDRefPID getFCardID(int Id)
        {
            FCardIDRefPID fCard = RAcontext.FCardIDRefPIDs.Find(Id);
            return fCard;
        }

        public int updateFCardID(int Id, string fCardID,string passport, bool isVisitor, bool isActive, int mode)
        {
            try
            {
                if (userNamecurr == null)
                {
                    return -3;
                }
                FCardIDRefPID fCard = RAcontext.FCardIDRefPIDs.Find(Id);
                if (mode == 2)
                {
                    fCard.FCardID = fCardID;
                    if (isVisitor)
                    {
                        fCard.Remark = "Visitor";
                        fCard.PassportID = passport;
                    }
                    else
                    {
                        fCard.Remark = "Member";
                        fCard.PassportID = "";
                    }
                    
                }
                else
                {
                    fCard.IsActive = isActive;
                }
              
                fCard.DateUpdated = DateTime.Now;
                fCard.UpdatedBy = userNamecurr;


                RAcontext.SaveChanges();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        //Add fake Card Id end

        
        
    }
}
