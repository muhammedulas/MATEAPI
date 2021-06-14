using MATE.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin;
using System.Web;
using System.Web.Http.Description;
using System.Collections;

namespace MATE.Controllers
{
    //[Authorize(Roles = "admin,user,depManager,compManager")]
    public class DefaultController : ApiController
    {
        readonly MATEEntities db = new MATEEntities();
        readonly responseModel resp = new responseModel();
        readonly HttpResponseMessage response = new HttpResponseMessage();
        readonly Md5Crypt hashTool = new Md5Crypt();

        #region Public Methods
        //Bu alandaki metodlarda kullanıcılar sadece kendilerine ait bilgileri alabilir (yetki gözetmeksizin).


        [HttpGet]
        [Route("api/chartData/{userId}")]
        public IHttpActionResult GetChartData(int userID)
        {
            List<chartDataModel> data = new List<chartDataModel>();
            chartDataModel byStatus = new chartDataModel();
            byStatus.dataDefinition = "Durumlarına Göre Görevlerim";
            List<string> byStatusLabels = new List<string>();
            List<int> byStatusSeries = new List<int>();
            byStatusLabels.Add("Beklemede");
            byStatusSeries.Add(db.TASKS.Where(q => q.ASSIGNED_USER == userID && q.STATUS == 0).Count());
            byStatusLabels.Add("Devam Ediyor");
            byStatusSeries.Add(db.TASKS.Where(q => q.ASSIGNED_USER == userID && q.STATUS == 1).Count());
            byStatusLabels.Add("Tamamlandı");
            byStatusSeries.Add(db.TASKS.Where(q => q.ASSIGNED_USER == userID && q.STATUS == 2).Count());
            byStatus.labels = byStatusLabels;
            byStatus.series = byStatusSeries;


            chartDataModel byResults = new chartDataModel();
            List<string> byResultsLabels = new List<string>();
            List<int> byResultsSeries = new List<int>();
            byResults.dataDefinition = "Sonuçlarına Göre Tamamlanan Görevler";
            byResultsLabels.Add("Başarılı");
            byResultsSeries.Add(db.TASKS.Where(q => q.ASSIGNED_USER == userID && q.RESULT == 1 && q.STATUS == 2).Count());
            byResultsLabels.Add("Başarısız");
            byResultsSeries.Add(db.TASKS.Where(q => q.ASSIGNED_USER == userID && q.RESULT == 2 && q.STATUS == 2).Count());
            byResultsLabels.Add("İptal Edildi");
            byResultsSeries.Add(db.TASKS.Where(q => q.ASSIGNED_USER == userID && q.RESULT == 3 && q.STATUS == 2).Count());
            byResults.series = byResultsSeries;
            byResults.labels = byResultsLabels;


            chartDataModel todos = new chartDataModel();
            List<string> todoLabels = new List<string>();
            List<int> todoSeries = new List<int>();
            todos.dataDefinition = "Yapılacaklar Listem";
            todoLabels.Add("Beklemede");
            todoSeries.Add(db.PERSONAL_TASKS.Where(q => q.STATUS == 0 && q.USERREF == userID).Count());
            todoLabels.Add("Tamamlandı");
            todoSeries.Add(db.PERSONAL_TASKS.Where(q=> q.STATUS == 1 && q.USERREF == userID).Count());

            todos.series = todoSeries;
            todos.labels = todoLabels;

            data.Add(byStatus);
            data.Add(byResults);
            data.Add(todos);
           
          
            return Ok(data);
        }

        //Kullanıcının görevlerini getirir
        [HttpGet]
        [Route("api/getTasks/{userId}")]
        public IHttpActionResult GetMyTasks(int userId)
        {
            var context = Request.GetOwinContext();
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "sub");
            if (userClaim.Value != db.USERS.SingleOrDefault(q => q.USERID == userId).USERNAME)
            {
                resp.OK = false;
                resp.message = "Kullanıcı doğrulanamadı";
                return BadRequest(resp.message);
            }
            List<taskModel> taskList = new List<taskModel>();
            List<taskModel> userTasks = db.TASKS.Where(q => q.TEAM_TASK == false && q.ASSIGNED_USER == userId).Select(t => new taskModel()
            {
                TASKID = t.TASKID,
                TEAM_TASK = t.TEAM_TASK,
                ASSIGNED_TEAM = t.ASSIGNED_TEAM,
                ASSIGNED_USER = t.ASSIGNED_USER,
                CREATED_BY = t.CREATED_BY,
                CREATED_AT = t.CREATED_AT,
                DEADLINE = t.DEADLINE,
                MODIFIED_BY = t.MODIFIED_BY,
                MODIFIED_AT = t.MODIFIED_AT,
                TASK_TITLE = t.TASK_TITLE,
                TASK_DESCRIPTION = t.TASK_DESCRIPTION,
                STATUS = t.STATUS,
                RESULT = t.RESULT,
                CLOSED_AT = t.CLOSED_AT
            }).ToList();
            List<VW_TEAMS> relatedTeams = db.VW_TEAMS.Where(q => q.USERID == userId).ToList();
            List<taskModel> teamTasks = new List<taskModel>();
            relatedTeams.ForEach(team =>
            {
                List<taskModel> tasks = db.TASKS.Where(q => q.TEAM_TASK == true && q.ASSIGNED_TEAM == team.TEAMID).Select(t => new taskModel()
                {
                    TASKID = t.TASKID,
                    TEAM_TASK = t.TEAM_TASK,
                    ASSIGNED_TEAM = t.ASSIGNED_TEAM,
                    ASSIGNED_USER = t.ASSIGNED_USER,
                    CREATED_BY = t.CREATED_BY,
                    CREATED_AT = t.CREATED_AT,
                    DEADLINE = t.DEADLINE,
                    MODIFIED_BY = t.MODIFIED_BY,
                    MODIFIED_AT = t.MODIFIED_AT,
                    TASK_TITLE = t.TASK_TITLE,
                    TASK_DESCRIPTION = t.TASK_DESCRIPTION,
                    STATUS = t.STATUS,
                    RESULT = t.RESULT,
                    CLOSED_AT = t.CLOSED_AT
                }).ToList();
                teamTasks.AddRange(tasks);
            });

            List<taskModel> createdByUser = db.TASKS.Where(q => q.CREATED_BY == userClaim.Value).Select(t => new taskModel() {
                TASKID = t.TASKID,
                TEAM_TASK = t.TEAM_TASK,
                ASSIGNED_TEAM = t.ASSIGNED_TEAM,
                ASSIGNED_USER = t.ASSIGNED_USER,
                CREATED_BY = t.CREATED_BY,
                CREATED_AT = t.CREATED_AT,
                DEADLINE = t.DEADLINE,
                MODIFIED_BY = t.MODIFIED_BY,
                MODIFIED_AT = t.MODIFIED_AT,
                TASK_TITLE = t.TASK_TITLE,
                TASK_DESCRIPTION = t.TASK_DESCRIPTION,
                STATUS = t.STATUS,
                RESULT = t.RESULT,
                CLOSED_AT = t.CLOSED_AT
            }).ToList();


            teamTasks.Distinct();
            userTasks.Distinct();
            createdByUser.Distinct();
            taskList.AddRange(teamTasks);
            taskList.AddRange(userTasks);
            taskList.AddRange(createdByUser);
            taskList.ForEach(t =>
            {
                if (t.TEAM_TASK == true)
                {
                    if (db.TEAMS.Count(q => q.TEAMID == t.ASSIGNED_TEAM) > 0)
                    {
                        t.ASSIGNED_TEAM_NAME = db.TEAMS.FirstOrDefault(q => q.TEAMID == t.ASSIGNED_TEAM).TEAM_NAME;
                    }

                }
                else
                {
                    if (db.USERS.Count(q => q.USERID == t.ASSIGNED_USER) > 0)
                    {
                        t.ASSIGNED_USER_USERNAME = db.USERS.FirstOrDefault(q => q.USERID == t.ASSIGNED_USER).USERNAME;
                    }

                }


            });
            taskList.Distinct();

            return Ok(taskList);

        }


        //Kullanıcıyı ilgilendiren anonsları getirir.
        [HttpGet]
        [Route("api/getAnnouncements/{userId}")]
        public IHttpActionResult GetAnnouncements(int userId)
        {
            var context = Request.GetOwinContext();
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "sub");

            if (userClaim.Value != db.USERS.SingleOrDefault(q => q.USERID == userId).USERNAME)
            {
                resp.OK = false;
                resp.message = "Kullanıcı doğrulanamadı";
                return BadRequest(resp.message);
            }

            announcementModel Announcements = new announcementModel();
            Announcements.TeamAnnouncements = new List<teamAnnouncementModel>();
            Announcements.CompanyAnnouncements = new List<companyAnnouncementModel>();
            List<VW_TEAMS> relatedTeams = new List<VW_TEAMS>();
            List<VW_DEPARTMENTS> relatedDepartments = new List<VW_DEPARTMENTS>();
            var test = new List<teamAnnouncementModel>();
            relatedTeams = db.VW_TEAMS.Where(q => q.USERID == userId).ToList();
            relatedTeams.ForEach(team =>
            {
                relatedDepartments.Add(db.VW_DEPARTMENTS.FirstOrDefault(q => q.DEPID == team.DEPREF));
                List<teamAnnouncementModel> teamAnnouncements = (List<teamAnnouncementModel>)db.TEAM_ANNOUNCEMENTS.Where(q => q.TEAM_ID == team.TEAMID).Select(rec => new teamAnnouncementModel()
                {
                    ANNID = rec.ANNID,
                    OWNER_ID = rec.OWNER_ID,
                    TEAM_ID = rec.TEAM_ID,
                    CREATED_AT = rec.CREATED_AT,
                    DEADLINE = rec.DEADLINE,
                    TITLE = rec.TITLE,
                    CONTENT = rec.CONTENT
                }).ToList();
                relatedDepartments.Add(db.VW_DEPARTMENTS.FirstOrDefault(q => q.DEPID == team.DEPREF));
                Announcements.TeamAnnouncements.AddRange(teamAnnouncements);

            });
            var deps = relatedDepartments.Distinct().ToList();
            List<companyModel> companies = new List<companyModel>();
            Announcements.DepartmentAnnouncements = new List<departmentAnnouncementModel>();

            deps.ForEach(dep =>
            {
                List<departmentAnnouncementModel> departmentAnnouncements = db.DEPARTMENT_ANNOUNCEMENTS.Where(q => q.DEPARTMENT_ID == dep.DEPID).Select(rec => new departmentAnnouncementModel()
                {
                    ANNID = rec.ANNID,
                    OWNER_ID = rec.OWNER_ID,
                    DEPARTMENT_ID = rec.DEPARTMENT_ID,
                    CREATED_AT = rec.CREATED_AT,
                    DEADLINE = rec.DEADLINE,
                    TITLE = rec.TITLE,
                    CONTENT = rec.CONTENT
                }).ToList();
                Announcements.DepartmentAnnouncements.AddRange(departmentAnnouncements);
                companies.Add(db.COMPANY_INFO.Where(q => q.COMPID == dep.COMPREF).Select(c => new companyModel()
                {
                    COMPID = c.COMPID,
                    COUNTRY = c.COUNTRY,
                    CITY = c.CITY,
                    DISTRICT = c.DISTRICT,
                    OPEN_ADRESS = c.OPEN_ADRESS,
                    POSTAL_CODE = c.POSTAL_CODE,
                    PHONE = c.PHONE,
                    TITLE = c.TITLE

                }).SingleOrDefault());
            });


            var comps = companies.Distinct().ToList();
            comps.ForEach(comp =>
            {
                List<companyAnnouncementModel> companyAnnouncements = db.COMPANY_ANNOUNCEMENTS.Where(q => q.COMPANY_ID == comp.COMPID).Select(rec => new companyAnnouncementModel()
                {
                    ANNID = rec.ANNID,
                    OWNER_ID = rec.OWNER_ID,
                    COMPANY_ID = rec.COMPANY_ID,
                    CREATED_AT = rec.CREATED_AT,
                    DEADLINE = rec.DEADLINE,
                    TITLE = rec.TITLE,
                    CONTENT = rec.CONTENT
                }).ToList();
                Announcements.CompanyAnnouncements.AddRange(companyAnnouncements);
            });

            return Ok(Announcements);
        }

        //Kullanıcının dahil olduğu ekipleri getirir.
        [HttpGet]
        [Route("api/getRelatedTeams/{userId}")]
        public IHttpActionResult GetRelatedTeams(int userId)
        {
            var context = Request.GetOwinContext();
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "sub");
            if (userClaim.Value != db.USERS.SingleOrDefault(q => q.USERID == userId).USERNAME)
            {
                resp.OK = false;
                resp.message = "Kullanıcı doğrulanamadı";
                return BadRequest(resp.message);
            }

            List<teamModel> teams = new List<teamModel>();
            List<TEAM_MEMBERS> relations = new List<TEAM_MEMBERS>();
            relations = db.TEAM_MEMBERS.Where(q => q.USERREF == userId).ToList();

            relations.ForEach(rel =>
            {
                var currentTeam = db.VW_TEAMS.FirstOrDefault(q => q.TEAMID == rel.TEAMREF);
                List<teamMemberModel> members = (List<teamMemberModel>)db.VW_TEAMS.Where(q => q.TEAMID == rel.TEAMREF).Select(rec => new teamMemberModel()
                {
                    NAME = rec.NAME,
                    SURNAME = rec.SURNAME,
                    USERID = rec.USERID,
                    USERNAME = rec.USERNAME,
                    Role = rec.ROLE
                }).ToList();
                teamModel data = new teamModel();
                data.TEAMID = currentTeam.TEAMID;
                data.DEPREF = currentTeam.DEPREF;
                data.TEAM_DEF = currentTeam.TEAM_DEF;
                data.TEAM_NAME = currentTeam.TEAM_NAME;
                data.MEMBERS = members;
                data.MEMBER_COUNT = db.TEAM_MEMBERS.Count(q=> q.TEAMREF == data.TEAMID);
                teams.Add(data);

            });

            return Ok(teams);
        }

        [HttpGet]
        [Route("api/getOwnUserInfo/{userId}")]
        public IHttpActionResult GetOwnUserInfo(int userId)
        {
            var context = Request.GetOwinContext();
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "sub");
            if (userClaim.Value != db.USERS.SingleOrDefault(q => q.USERID == userId).USERNAME)
            {
                resp.OK = false;
                resp.message = "Kullanıcı doğrulanamadı";
                return BadRequest(resp.message);
            }

            var userInfo = new userModel();
            userInfo.CONTACT_INFO = new userContactInfo();
            var recUser = db.USERS.SingleOrDefault(q => q.USERID == userId);
            var contactInfo = db.USER_CONTACT_INFO.Where(q => q.USERREF == userId).Select(inf => new userContactInfo
            {
                ID = inf.ID,
                USERREF = inf.USERREF,
                PHONE = inf.PHONE,
                MAIL = inf.MAIL,
                COUNTRY = inf.COUNTRY,
                CITY = inf.CITY,
                DISTRICT = inf.DISTRICT,
                POSTAL_CODE = inf.POSTAL_CODE,
                OPEN_ADRESS = inf.OPEN_ADRESS,
                PROFILE_PHOTO = inf.PROFILE_PHOTO

            }).SingleOrDefault();

            userInfo.USERID = recUser.USERID;
            userInfo.USERNAME = recUser.USERNAME;
            userInfo.PW_KEY = recUser.PW_KEY;
            userInfo.STATUS = recUser.STATUS;
            userInfo.IS_ADMIN = recUser.IS_ADMIN;
            userInfo.NAME = recUser.NAME;
            userInfo.SURNAME = recUser.SURNAME;
            userInfo.CONTACT_INFO = contactInfo;



            return Ok(userInfo);
        }

        #endregion

        #region Announcements

        [Authorize(Roles = "admin,companyManager")]
        #region CompanyLevel

        [HttpGet]
        [Route("api/announcements/company")]
        public IHttpActionResult GetCompAnnouncements()
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 24) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }

            List<companyAnnouncementModel> annList = new List<companyAnnouncementModel>();
            annList = db.COMPANY_ANNOUNCEMENTS.Select(q => new companyAnnouncementModel()
            {
                ANNID = q.ANNID,
                OWNER_ID = q.OWNER_ID,
                COMPANY_ID = q.COMPANY_ID,
                CREATED_AT = q.CREATED_AT,
                DEADLINE = q.DEADLINE,
                TITLE = q.TITLE,
                CONTENT = q.CONTENT
            }).ToList();
            return Ok(annList);
        }

        [HttpGet]
        [Route("api/announcements/company/{annId}")]
        public IHttpActionResult GetCompAnnouncement(int annId)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 24) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            if (db.COMPANY_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return NotFound();
            }
            return Ok(db.COMPANY_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == annId));
        }


        [HttpPost]
        [Route("api/announcements/company/makeAnnouncement")]
        public IHttpActionResult MakeCompAnn(COMPANY_ANNOUNCEMENTS announcement)
        {
            ////var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            ////var userId = Convert.ToInt32(userClaim.Value);
            ////if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 25) < 1)
            ////{
            ////    resp.OK = false;
            ////    resp.message = "Bu işlem için yetkiniz yok!";
            ////}
            db.COMPANY_ANNOUNCEMENTS.Add(announcement);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Duyuru başarıyla yayınlandı";
            return Ok(resp);
        }

        [HttpPut]
        [Route("api/announcements/company/{annId}")]
        public IHttpActionResult UpdateCompAnn(COMPANY_ANNOUNCEMENTS announcement, int annId)
        {
            //var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            //var userId = Convert.ToInt32(userClaim.Value);
            //if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 26) < 1)
            //{
            //    resp.OK = false;
            //    resp.message = "Bu işlem için yetkiniz yok!";
            //}
            if (db.COMPANY_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return BadRequest(resp.message);
            }

            COMPANY_ANNOUNCEMENTS record = db.COMPANY_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == announcement.ANNID);
            record = announcement;
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Duyuru başarıyla güncellendi";
            return Ok(resp);
        }

        [HttpDelete]
        [Route("api/announcements/company/{annId}")]
        public IHttpActionResult DeleteCompAnnouncement(int annId)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 27) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            if (db.COMPANY_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return BadRequest(resp.message);
            }
            else
            {
                var record = db.COMPANY_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == annId);
                db.COMPANY_ANNOUNCEMENTS.Remove(record);
                db.SaveChanges();
                resp.OK = true;
                resp.message = "Duyuru başarıyla silindi";
                return Ok(resp);
            }
        }

        #endregion

        #region Department Level

        [HttpGet]
        [Route("api/announcements/department")]
        public IHttpActionResult GetDepAnnouncements()
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 28) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            List<departmentAnnouncementModel> annList = new List<departmentAnnouncementModel>();
            annList = db.DEPARTMENT_ANNOUNCEMENTS.Select(q => new departmentAnnouncementModel()
            {
                ANNID = q.ANNID,
                OWNER_ID = q.OWNER_ID,
                DEPARTMENT_ID = q.DEPARTMENT_ID,
                CREATED_AT = q.CREATED_AT,
                DEADLINE = q.DEADLINE,
                TITLE = q.TITLE,
                CONTENT = q.CONTENT
            }).ToList();
            return Ok(annList);
        }

        [HttpGet]
        [Route("api/announcements/department/{annId}")]
        public IHttpActionResult GetDepAnnouncement(int annId)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 28) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            if (db.DEPARTMENT_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return NotFound();
            }
            return Ok(db.DEPARTMENT_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == annId));
        }


        [HttpPost]
        [Route("api/announcements/department/makeAnnouncement")]
        public IHttpActionResult MakeDepAnn(DEPARTMENT_ANNOUNCEMENTS announcement)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 30) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            db.DEPARTMENT_ANNOUNCEMENTS.Add(announcement);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Duyuru başarıyla yayınlandı";
            return Ok(resp);
        }

        [HttpPut]
        [Route("api/announcements/department/{annId}")]
        public IHttpActionResult UpdateDepAnn(DEPARTMENT_ANNOUNCEMENTS announcement, int annId)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 31) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            if (db.DEPARTMENT_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return BadRequest(resp.message);
            }

            DEPARTMENT_ANNOUNCEMENTS record = db.DEPARTMENT_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == announcement.ANNID);
            record = announcement;
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Duyuru başarıyla güncellendi";
            return Ok(resp);
        }

        [HttpDelete]
        [Route("api/announcements/department/{annId}")]
        public IHttpActionResult DeleteDepAnnouncement(int annId)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 32) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            if (db.DEPARTMENT_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return BadRequest(resp.message);
            }
            else
            {
                var record = db.DEPARTMENT_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == annId);
                db.DEPARTMENT_ANNOUNCEMENTS.Remove(record);
                db.SaveChanges();
                resp.OK = true;
                resp.message = "Duyuru başarıyla silindi";
                return Ok(resp);
            }
        }

        #endregion

        #region Team Level

        [HttpGet]
        [Route("api/announcements/team")]
        public IHttpActionResult GetTeamAnnouncements()
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 33) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
                return BadRequest(resp.message);
            }
            List<teamAnnouncementModel> annList = new List<teamAnnouncementModel>();
            annList = db.TEAM_ANNOUNCEMENTS.Select(q => new teamAnnouncementModel() {
                ANNID = q.ANNID,
                OWNER_ID = q.OWNER_ID,
                TEAM_ID = q.TEAM_ID,
                CREATED_AT = q.CREATED_AT,
                DEADLINE = q.DEADLINE,
                TITLE = q.TITLE,
                CONTENT = q.CONTENT
            }).ToList();
            return Ok(annList);
        }

        [HttpGet]
        [Route("api/announcements/team/{annId}")]
        public IHttpActionResult GetTeamAnnouncement(int annId)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 33) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            if (db.TEAM_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return NotFound();
            }


            return Ok(db.TEAM_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == annId));
        }


        [HttpPost]
        [Route("api/announcements/team/makeAnnouncement")]
        public IHttpActionResult MakeTeamAnn(TEAM_ANNOUNCEMENTS announcement)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 34) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            db.TEAM_ANNOUNCEMENTS.Add(announcement);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Duyuru başarıyla yayınlandı";
            return Ok(resp);
        }

        [HttpPut]
        [Route("api/announcements/team/{annId}")]
        public IHttpActionResult UpdateTeamAnn(TEAM_ANNOUNCEMENTS announcement, int annId)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 35) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            if (db.TEAM_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return BadRequest(resp.message);
            }

            TEAM_ANNOUNCEMENTS record = db.TEAM_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == announcement.ANNID);
            record = announcement;
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Duyuru başarıyla güncellendi";
            return Ok(resp);
        }

        [HttpDelete]
        [Route("api/announcements/team/{annId}")]
        public IHttpActionResult DeleteTeamAnnouncement(int annId)
        {
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            var userId = Convert.ToInt32(userClaim.Value);
            if (db.USER_ROLES.Count(q => q.USERREF == userId && q.ROLEREF == 36) < 1)
            {
                resp.OK = false;
                resp.message = "Bu işlem için yetkiniz yok!";
            }
            if (db.TEAM_ANNOUNCEMENTS.Count(q => q.ANNID == annId) < 1)
            {
                resp.OK = false;
                resp.message = "Kayıt Bulunamadı";
                return BadRequest(resp.message);
            }
            else
            {
                var record = db.TEAM_ANNOUNCEMENTS.SingleOrDefault(q => q.ANNID == annId);
                db.TEAM_ANNOUNCEMENTS.Remove(record);
                db.SaveChanges();
                resp.OK = true;
                resp.message = "Duyuru başarıyla silindi";
                return Ok(resp);
            }
        }

        #endregion

        #endregion

        #region User Management
        //[Authorize(Roles = "admin,CompanyManager")]
        [HttpGet]
        [Route("api/users")]
        [ResponseType(typeof(List<USERS>))]
        public List<userModel> GetUsers(bool detail = false)
        {


            List<userModel> userList = db.USERS.Select(u => new userModel()
            {
                USERID = u.USERID,
                USERNAME = u.USERNAME,
                PW_KEY = u.PW_KEY,
                NAME = u.NAME,
                SURNAME = u.SURNAME,
                IS_ADMIN = u.IS_ADMIN,
                STATUS = u.STATUS

            }).ToList();

            if (detail == true)
            {
                userList.ForEach(user =>
                {
                    user.CONTACT_INFO = new userContactInfo();
                    var contactInfo = db.USER_CONTACT_INFO.Where(q => q.USERREF == user.USERID).Select(inf => new userContactInfo
                    {
                        ID = inf.ID,
                        USERREF = inf.USERREF,
                        PHONE = inf.PHONE,
                        MAIL = inf.MAIL,
                        COUNTRY = inf.COUNTRY,
                        CITY = inf.CITY,
                        DISTRICT = inf.DISTRICT,
                        POSTAL_CODE = inf.POSTAL_CODE,
                        OPEN_ADRESS = inf.OPEN_ADRESS

                    }).FirstOrDefault();
                    user.CONTACT_INFO = contactInfo;

                });
            }
            return userList;

        }
        [HttpGet]
        [Route("api/users/{userId}")]
        public IHttpActionResult GetUser(int userId, bool? detail = false)
        {

            if (db.USERS.Count(q => q.USERID == userId) < 1)
            {
                resp.OK = false;
                resp.message = "Kullanıcı Bulunamadı";
                return NotFound();

            }
            userModel user = (userModel)db.USERS.Where(u => u.USERID == userId).Select(u => new userModel()
            {
                USERID = u.USERID,
                USERNAME = u.USERNAME,
                PW_KEY = u.PW_KEY,
                NAME = u.NAME,
                SURNAME = u.SURNAME,
                IS_ADMIN = u.IS_ADMIN,
                STATUS = u.STATUS,

            }).SingleOrDefault();
            if (detail == true)
            {
                var contactInfo = db.USER_CONTACT_INFO.Where(q => q.USERREF == userId).Select(inf => new userContactInfo
                {
                    ID = inf.ID,
                    USERREF = inf.USERREF,
                    PHONE = inf.PHONE,
                    MAIL = inf.MAIL,
                    COUNTRY = inf.COUNTRY,
                    CITY = inf.CITY,
                    DISTRICT = inf.DISTRICT,
                    POSTAL_CODE = inf.POSTAL_CODE,
                    OPEN_ADRESS = inf.OPEN_ADRESS

                }).SingleOrDefault();
                user.CONTACT_INFO = contactInfo;
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("api/users")]
        [ResponseType(typeof(USERS))]
        public IHttpActionResult AddUser(userModel user)
        {
            if (db.USERS.Count(val => val.USERNAME == user.USERNAME) > 0)
            {
                resp.OK = false;
                resp.message = "Sistemde aynı kullanıcı adı ile kayıt mevcut";
                return BadRequest(resp.message);
            };
            user.PW_KEY = hashTool.CalculateHash(user.PW_KEY);
            var userInfo = new USERS
            {
                IS_ADMIN = user.IS_ADMIN,
                USERNAME = user.USERNAME,
                PW_KEY = user.PW_KEY,
                SURNAME = user.SURNAME,
                NAME = user.NAME,
                STATUS = user.STATUS,
                USERID = 0
            };
            db.USERS.Add(userInfo);
            db.SaveChanges();
            var userRef = db.USERS.FirstOrDefault(q => q.USERNAME == userInfo.USERNAME).USERID;
            var contactInfo = new USER_CONTACT_INFO
            {
                USERREF = userRef,
                PHONE = user.CONTACT_INFO.PHONE,
                MAIL = user.CONTACT_INFO.MAIL,
                COUNTRY = user.CONTACT_INFO.COUNTRY,
                CITY = user.CONTACT_INFO.CITY,
                DISTRICT = user.CONTACT_INFO.DISTRICT,
                POSTAL_CODE = user.CONTACT_INFO.POSTAL_CODE,
                OPEN_ADRESS = user.CONTACT_INFO.OPEN_ADRESS,
                PROFILE_PHOTO = user.CONTACT_INFO.PROFILE_PHOTO
            };
            db.USER_CONTACT_INFO.Add(contactInfo);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt başarılı";
            return Ok(resp);
        }

        [HttpGet]
        [Route("api/setPassword")]
        public IHttpActionResult setPassword(int id = 0, string password = "")
        {
            if (id == 0 || password == "")
            {
                resp.OK = false;
                resp.message = "Eksik bilgi girişi.";
                return BadRequest(resp.message);
            }
            var context = Request.GetOwinContext();
            var adminClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "isAdmin");
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "sub");
            if (userClaim.Value != db.USERS.FirstOrDefault(q => q.USERID == id).USERNAME  && adminClaim.Value == "false")
            {
                resp.OK = false;
                resp.message = "Kullanıcı doğrulanamadı";
                return BadRequest(resp.message);
            }

            var encypted = hashTool.CalculateHash(password);
            var user = db.USERS.SingleOrDefault(q => q.USERID == id);
            user.PW_KEY = encypted;

            db.SaveChanges();
            resp.OK = true;
            resp.message = "Parola güncellendi";

            return Ok(resp);
        }


        [HttpPut]
        [Route("api/users/{userId}")]
        public IHttpActionResult UpdateUser(USERS user, int userId)
        {
            var record = db.USERS.SingleOrDefault(r => r.USERID == userId);
            record.USERNAME = user.USERNAME;
            record.PW_KEY = user.PW_KEY;
            record.NAME = user.NAME;
            record.SURNAME = user.SURNAME;
            record.IS_ADMIN = user.IS_ADMIN;
            record.STATUS = user.STATUS;
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt başarıyla güncellendi";
            return Ok(resp);
        }

        [HttpDelete]
        [Route("api/users/{userId}")]
        public IHttpActionResult DeleteUser(int userId)
        {
            if (db.USERS.Count(rec => rec.USERID == userId) == 0)
            {
                resp.OK = false;
                resp.message = "Kullanıcı Bulunamadı";
                return NotFound();
            }
            if(db.USERS.SingleOrDefault(q=> q.USERID == userId).IS_ADMIN == true && db.USERS.Count(q => q.IS_ADMIN == true) < 2)
            {
                resp.OK = false;
                resp.message = "Sistemde en az bir yönetici bulunmalıdır.";
                return BadRequest(resp.message);
            }
            else
            {
                var recUsers = db.USERS.Where(q => q.USERID == userId).SingleOrDefault();
                db.USERS.Remove(recUsers);
                var userRoles = db.USER_ROLES.Where(q => q.USERREF == userId).ToList();
                db.USER_ROLES.RemoveRange(userRoles);
                var recTeamRels = db.TEAM_MEMBERS.Where(q => q.USERREF == userId).ToList();
                db.TEAM_MEMBERS.RemoveRange(recTeamRels);
                var recTasks = db.TASKS.Where(q => q.ASSIGNED_USER == userId).ToList();
                db.TASKS.RemoveRange(recTasks);
                var recPersTasks = db.PERSONAL_TASKS.Where(q => q.USERREF == userId).ToList();
                db.PERSONAL_TASKS.RemoveRange(recPersTasks);
                var recContactInfo = new USER_CONTACT_INFO();
                recContactInfo = db.USER_CONTACT_INFO.Where(q => q.USERREF == userId).FirstOrDefault();
                db.USER_CONTACT_INFO.Remove(recContactInfo);
                var recCompAnn = db.COMPANY_ANNOUNCEMENTS.Where(q => q.OWNER_ID == userId).ToList();
                db.COMPANY_ANNOUNCEMENTS.RemoveRange(recCompAnn);
                var recDepAnn = db.DEPARTMENT_ANNOUNCEMENTS.Where(q => q.OWNER_ID == userId).ToList();
                db.DEPARTMENT_ANNOUNCEMENTS.RemoveRange(recDepAnn);
                var recTeamAnn = db.TEAM_ANNOUNCEMENTS.Where(q => q.OWNER_ID == userId).ToList();
                db.TEAM_ANNOUNCEMENTS.RemoveRange(recTeamAnn);
                db.PERSONAL_TASKS.RemoveRange(recPersTasks);
                db.SaveChanges();
                resp.OK = true;
                resp.message = "Kullanıcıya ait bütün veriler silindi";
                return Ok(resp.message);
            }
        }

        #endregion

        #region Unit Management

        #region Company

        [HttpPost]
        [Route("api/companies")]
        public IHttpActionResult AddCompany(COMPANY_INFO company)
        {
            if (db.COMPANY_INFO.Count(q => q.TITLE == company.TITLE) > 0)
            {
                resp.OK = false;
                resp.message = "Aynı isme sahip bir firma var!";
                return BadRequest(resp.message);
            }
            db.COMPANY_INFO.Add(company);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt başarılı";
            return Ok(resp.message);
        }

        [HttpPut]
        [Route("api/companies/{compId}")]
        public IHttpActionResult UpdateCompany(COMPANY_INFO company, int compId)
        {
            if (db.COMPANY_INFO.SingleOrDefault(q => q.COMPID == compId) == null)
            {
                return BadRequest("Kayıt Bulunamadı");
            }
            else
            {
                var record = db.COMPANY_INFO.SingleOrDefault(r => r.COMPID == company.COMPID);
                record.TITLE = company.TITLE;
                record.COMMERCIAL_TITLE = company.COMMERCIAL_TITLE;
                record.COUNTRY = company.COUNTRY;
                record.CITY = company.CITY;
                record.DISTRICT = company.DISTRICT;
                record.OPEN_ADRESS = company.OPEN_ADRESS;
                record.POSTAL_CODE = company.POSTAL_CODE;
                record.PHONE = company.PHONE;
                record.MAIL = company.MAIL;
                db.SaveChanges();
                return Ok("Kayıt Başarıyla Güncellendi");
            }
        }

        [HttpGet]
        [Route("api/companies")]
        public List<companyModel> GetCompanies(bool detail = false)
        {
            List<companyModel> compList = (List<companyModel>)db.COMPANY_INFO.Select(company => new companyModel
            {
                COMPID = company.COMPID,
                TITLE = company.TITLE,
                COMMERCIAL_TITLE = company.COMMERCIAL_TITLE,
                COUNTRY = company.COUNTRY,
                CITY = company.CITY,
                DISTRICT = company.DISTRICT,
                OPEN_ADRESS = company.OPEN_ADRESS,
                POSTAL_CODE = company.POSTAL_CODE,
                PHONE = company.PHONE,
                MAIL = company.MAIL
            }).ToList();

            if (detail == true)
            {
                compList.ForEach(comp =>
                {
                    comp.DEPARTMENTS = new List<departmentModel>();
                    comp.DEPARTMENTS = (List<departmentModel>)db.DEPARTMENTS.Where(q => q.COMPREF == comp.COMPID).Select(dep => new departmentModel()
                    {
                        DEPID = dep.DEPID,
                        DEPARTMENT_DEF = dep.DEPARTMENT_DEF,
                        DEPARTMENT_NAME = dep.DEPARTMENT_NAME

                    }).ToList();
                });
            }
            return compList;
        }

        [HttpGet]
        [Route("api/companies/{compId}")]
        public IHttpActionResult GetCompany(int compId, bool detail = false)
        {
            if (db.COMPANY_INFO.Count(q => q.COMPID == compId) < 1)
            {
                return NotFound();
            }
            companyModel company = new companyModel();
            company = db.COMPANY_INFO.Where(q => q.COMPID == compId).Select(val => new companyModel
            {
                COMPID = val.COMPID,
                TITLE = val.TITLE,
                COMMERCIAL_TITLE = val.COMMERCIAL_TITLE,
                COUNTRY = val.COUNTRY,
                CITY = val.CITY,
                DISTRICT = val.DISTRICT,
                OPEN_ADRESS = val.OPEN_ADRESS,
                POSTAL_CODE = val.POSTAL_CODE,
                PHONE = val.PHONE,
                MAIL = val.MAIL
            }).SingleOrDefault();
            if (detail == true)
            {
                company.DEPARTMENTS = new List<departmentModel>();
                company.DEPARTMENTS = db.DEPARTMENTS.Where(q => q.COMPREF == compId).Select(dep => new departmentModel()
                {
                    DEPID = dep.DEPID,
                    DEPARTMENT_DEF = dep.DEPARTMENT_DEF,
                    DEPARTMENT_NAME = dep.DEPARTMENT_NAME

                }).ToList();
            }
            return Ok(company);
        }

        [HttpDelete]
        [Route("api/companies/{compId}")]
        public IHttpActionResult DeleteCompany(int compId)
        {
            List<DEPARTMENTS> departments = new List<DEPARTMENTS>();
            departments = (List<DEPARTMENTS>)db.DEPARTMENTS.Where(q => q.COMPREF == compId).Select(dep => new DEPARTMENTS()
            {
                DEPID = dep.DEPID,
                DEPARTMENT_DEF = dep.DEPARTMENT_DEF,
                DEPARTMENT_NAME = dep.DEPARTMENT_NAME,
                COMPREF = dep.COMPREF
            }).ToList();

            departments.ForEach(dep =>
            {
                List<TEAMS> teams = new List<TEAMS>();
                teams = (List<TEAMS>)db.TEAMS.Where(q => q.DEPREF == dep.DEPID).Select(team => new TEAMS()
                {
                    TEAMID = team.TEAMID,
                    TEAM_DEF = team.TEAM_DEF,
                    TEAM_NAME = team.TEAM_NAME,
                    DEPREF = team.DEPREF
                });

                teams.ForEach(team =>
                {
                    List<TEAM_MEMBERS> relations = new List<TEAM_MEMBERS>();
                    relations = db.TEAM_MEMBERS.Where(q => q.TEAMREF == team.TEAMID).Select(rel => new TEAM_MEMBERS()
                    {
                        RELID = rel.RELID,
                        USERREF = rel.USERREF,
                        TEAMREF = rel.TEAMREF,
                        ROLE = rel.ROLE
                    }).ToList();
                    db.TEAM_MEMBERS.RemoveRange(relations);
                });
                db.TEAMS.RemoveRange(teams);
            });
            db.DEPARTMENTS.RemoveRange(departments);

            List<COMPANY_ANNOUNCEMENTS> announcements = new List<COMPANY_ANNOUNCEMENTS>();
            announcements = db.COMPANY_ANNOUNCEMENTS.Where(q => q.COMPANY_ID == compId).Select(ann => new COMPANY_ANNOUNCEMENTS()
            {
                ANNID = ann.ANNID,
                OWNER_ID = ann.OWNER_ID,
                COMPANY_ID = ann.COMPANY_ID,
                CREATED_AT = ann.CREATED_AT,
                DEADLINE = ann.DEADLINE,
                TITLE = ann.TITLE,
                CONTENT = ann.CONTENT
            }).ToList();
            db.COMPANY_ANNOUNCEMENTS.RemoveRange(announcements);

            return Ok("Firma Başarıyla Silindi");
        }
        #endregion

        #region Department
        [Authorize(Roles = "admin,companyManager")]
        [HttpGet]
        [Route("api/departments")]
        public IHttpActionResult GetDepartments(bool detail = false)
        {
            List<departmentModel> departments = new List<departmentModel>();
            departments = (List<departmentModel>)db.DEPARTMENTS.Select(dep => new departmentModel()
            {
                DEPID = dep.DEPID,
                COMPREF = dep.COMPREF,
                DEPARTMENT_NAME = dep.DEPARTMENT_NAME,
                DEPARTMENT_DEF = dep.DEPARTMENT_DEF
            }).ToList();

            if (detail == true)
            {
                departments.ForEach(department =>
                {
                    department.TEAMS = new List<teamModel>();
                    department.TEAMS = db.TEAMS.Where(q => q.DEPREF == department.DEPID).Select(team => new teamModel()
                    {
                        TEAMID = team.TEAMID,
                        DEPREF = team.DEPREF,
                        TEAM_NAME = team.TEAM_NAME,
                        TEAM_DEF = team.TEAM_DEF,
                        MEMBER_COUNT = db.TEAM_MEMBERS.Count(q=> q.TEAMREF == team.TEAMID)
                    }).ToList();
                });
            }
            return Ok(departments);
        }

        [HttpGet]
        [Route("api/departments/{depId}")]
        public IHttpActionResult GetDepartment(int depId, bool detail = false)
        {
            if (db.DEPARTMENTS.Count(q => q.DEPID == depId) < 1)
            {
                return NotFound();
            }

            departmentModel department = new departmentModel();
            department = (departmentModel)db.DEPARTMENTS.Where(q => q.DEPID == depId).Select(dep => new departmentModel()
            {
                DEPID = dep.DEPID,
                COMPREF = dep.COMPREF,
                DEPARTMENT_NAME = dep.DEPARTMENT_NAME,
                DEPARTMENT_DEF = dep.DEPARTMENT_DEF
            }).SingleOrDefault();

            if (detail == true)
            {
                department.TEAMS = new List<teamModel>();
                List<teamModel> teams = new List<teamModel>();
                teams = (List<teamModel>)db.TEAMS.Where(q => q.DEPREF == depId).Select(team => new teamModel()
                {
                    TEAMID = team.TEAMID,
                    DEPREF = team.DEPREF,
                    TEAM_NAME = team.TEAM_NAME,
                    TEAM_DEF = team.TEAM_DEF
                }).ToList();
            }
            return Ok(department);
        }

        [HttpPost]
        [Route("api/departments")]
        public IHttpActionResult AddDepartment(DEPARTMENTS department)
        {
            if (db.COMPANY_INFO.Count(q => q.COMPID == department.COMPREF) < 1)
            {
                resp.OK = false;
                resp.message = department.COMPREF.ToString() + " Numaralı firma bulunamadı";
                return BadRequest(resp.message);
            }

            if (db.DEPARTMENTS.Count(q => q.DEPARTMENT_NAME == department.DEPARTMENT_NAME) > 0)
            {
                resp.OK = false;
                resp.message = "Aynı isme sahip başka bir departman var!";
                return BadRequest(resp.message);
            }

            db.DEPARTMENTS.Add(department);
            db.SaveChanges();
            return Ok("Kayıt Başarıyla Eklendi");
        }

        [HttpPut]
        [Route("api/departments/{depId}")]
        public IHttpActionResult UpdateDepartment(DEPARTMENTS department, int depId)
        {
            var record = db.DEPARTMENTS.SingleOrDefault(r => r.DEPID == depId);
            record.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
            record.DEPARTMENT_DEF = department.DEPARTMENT_DEF;
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt Başarıyla güncellendi.";
            return Ok(resp);
        }

        [HttpDelete]
        [Route("api/departments/{depId}")]
        public IHttpActionResult DeleteDepartment(int depId)
        {
            var department = db.DEPARTMENTS.SingleOrDefault(r => r.DEPID == depId);
            if (db.TEAMS.Count(q=> q.DEPREF == depId) > 0)
            {
                resp.OK = false;
                resp.message = "Departmana Bağlı Ekip bulunuyor! Önce Ekipler Çıkarılmalıdır.";
                return BadRequest(resp.message);
            }
            else
            {
                db.DEPARTMENTS.Remove(department);
                db.SaveChanges();
                resp.OK = true;
                resp.message = "Kayıt başarıyla silindi";
                return Ok(resp);
            }

        }


        #endregion

        #region Team
        [Authorize(Roles = "admin,companyManager,departmentManager")]
        [HttpPost]
        [Route("api/teams")]
        public IHttpActionResult AddTeam(TEAMS team)
        {
            if (db.TEAMS.Count(q => q.TEAM_NAME == team.TEAM_NAME) > 0)
            {
                resp.OK = false;
                resp.message = "Aynı isme sahip başka bir ekip var";
                return BadRequest(resp.message);
            }

            if (db.DEPARTMENTS.Count(q => q.DEPID == team.DEPREF) < 1)
            {
                resp.OK = false;
                resp.message = team.DEPREF.ToString() + " Numaralı departman bulunamadı";
                return BadRequest(resp.message);
            }


            db.TEAMS.Add(team);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt Başarılı";
            return Ok(resp.message);
        }

        [HttpGet]
        [Route("api/teams")]
        public IHttpActionResult GetTeams(bool detail = false)
        {
            List<teamModel> teams = new List<teamModel>();
            teams = db.TEAMS.Select(team => new teamModel()
            {
                TEAMID = team.TEAMID,
                DEPREF = team.DEPREF,
                DEP_NAME = db.DEPARTMENTS.FirstOrDefault(q => q.DEPID == team.DEPREF).DEPARTMENT_NAME,
                TEAM_DEF = team.TEAM_DEF,
                TEAM_NAME = team.TEAM_NAME,
                MEMBER_COUNT = db.TEAM_MEMBERS.Count(q => q.TEAMREF == team.TEAMID)
            }).ToList();

            if (detail == true)
            {
                teams.ForEach(team =>
                {
                    team.MEMBERS = new List<teamMemberModel>();
                    team.MEMBERS = (List<teamMemberModel>)db.VW_TEAMS.Where(q => q.TEAMID == team.TEAMID).Select(member => new teamMemberModel()
                    {
                        USERID = member.USERID,
                        USERNAME = member.USERNAME,
                        NAME = member.NAME,
                        SURNAME = member.SURNAME,
                        Role = member.ROLE
                    }).OrderBy(q => q.Role).ToList();

                });
            }
            resp.OK = true;

            return Ok(teams);

        }

        [HttpGet]
        [Route("api/teams/{teamId}")]
        public IHttpActionResult GetTeam(int teamId, bool detail = false)
        {
            if (db.TEAMS.Count(q => q.TEAMID == teamId) < 1)
            {
                resp.OK = false;
                resp.message = teamId + " numaralı ekip bulunamadı!";
                return BadRequest(resp.message);
            }

            teamModel team = new teamModel();
            team = (teamModel)db.TEAMS.Where(q => q.TEAMID == teamId).Select(t => new teamModel
            {
                TEAMID = t.TEAMID,
                TEAM_DEF = t.TEAM_DEF,
                DEPREF = t.DEPREF,
                TEAM_NAME = t.TEAM_NAME
            }).SingleOrDefault();
            if (detail == true)
            {
                team.MEMBERS = new List<teamMemberModel>();
                team.MEMBERS = (List<teamMemberModel>)db.VW_TEAMS.Where(q => q.TEAMID == team.TEAMID).Select(member => new teamMemberModel()
                {
                    USERID = member.USERID,
                    USERNAME = member.USERNAME,
                    NAME = member.NAME,
                    SURNAME = member.SURNAME,
                    Role = member.ROLE
                }).ToList();
            }
            return Ok(team);
        }

        [HttpPut]
        [Route("api/teams/{teamId}")]
        public IHttpActionResult UpdateTeam(TEAMS team, int teamId)
        {
            if (db.TEAMS.Count(q => q.TEAMID == teamId) < 1)
            {
                resp.OK = false;
                resp.message = teamId + " numaralı ekip bulunamadı!";
                return BadRequest(resp.message);
            }

            TEAMS record = new TEAMS();
            record = db.TEAMS.SingleOrDefault(q => q.TEAMID == teamId);
            record.DEPREF = team.DEPREF;
            record.TEAM_DEF = team.TEAM_DEF;
            record.TEAM_NAME = team.TEAM_NAME;
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt başarıyla güncellendi.";
            return Ok(resp);
        }

        [HttpDelete]
        [Route("api/teams/{teamId}")]
        public IHttpActionResult DeleteTeam(int teamId)
        {
            if (db.TEAMS.Count(q => q.TEAMID == teamId) < 1)
            {
                resp.OK = false;
                resp.message = teamId + " numaralı kayıt bulunamadı";
                return BadRequest(resp.message);
            }

            List<TEAM_MEMBERS> relations = new List<TEAM_MEMBERS>();
            relations = db.TEAM_MEMBERS.Where(q => q.TEAMREF == teamId).ToList();
            db.TEAM_MEMBERS.RemoveRange(relations);
            TEAMS team = db.TEAMS.SingleOrDefault(q => q.TEAMID == teamId);
            db.TEAMS.Remove(team);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt başarıyla silindi.";
            return Ok(resp);
        }

        [HttpPost]
        [Route("api/teams/{teamId}/addUser({userId},{role})")]
        public IHttpActionResult AddUserToTeam(int teamId, int userId, int role)
        {
            if (db.USERS.SingleOrDefault(q => q.USERID == userId).STATUS == 0)
            {
                return BadRequest("Bloke durumdaki kullanıcılar üzerinde işlem yapılamaz");
            }
            if (db.USERS.Count(q => q.USERID == userId) < 1)
            {
                return BadRequest("Kullanıcı Bulunamadı!");
            }

            if (db.TEAMS.Count(q => q.TEAMID == teamId) < 1)
            {
                return BadRequest("Ekip Bulunamadı!");
            }
            if (db.TEAM_MEMBERS.Count(q => q.TEAMREF == teamId && q.USERREF == userId) > 0)
            {
                return BadRequest("Kullanıcı zaten bu ekibe dahil!");
            }
            TEAM_MEMBERS relation = new TEAM_MEMBERS();
            relation.USERREF = userId;
            relation.TEAMREF = teamId;
            relation.ROLE = role;
            db.TEAM_MEMBERS.Add(relation);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kullanıcı ekibe dahil edildi.";
            return Ok(resp);

        }

        [HttpDelete]
        [Route("api/teams/{teamId}/dismissUser({userID})")]
        public IHttpActionResult DismissUserFromTeam(int teamId, int userId)
        {
            if (db.USERS.Count(q => q.USERID == userId) < 1)
            {
                return BadRequest("Kullanıcı Bulunamadı!");
            }

            if (db.TEAMS.Count(q => q.TEAMID == teamId) < 1)
            {
                return BadRequest("Ekip Bulunamadı!");
            }
            if (db.TEAM_MEMBERS.Count(q => q.TEAMREF == teamId && q.USERREF == userId) < 1)
            {
                return BadRequest("Bu ekipte böyle bir kullanıcı yok!");
            }

            TEAM_MEMBERS relation = new TEAM_MEMBERS();
            relation = db.TEAM_MEMBERS.SingleOrDefault(q => q.USERREF == userId && q.TEAMREF == teamId);
            db.TEAM_MEMBERS.Remove(relation);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kullanıcı ekipten çıkarıldı.";
            return Ok(resp);
        }


        #endregion

        #endregion

        #region Task Management
        [Authorize(Roles = "admin, depManager, compManager")]
        [HttpGet]
        [Route("api/tasks")]
        public IHttpActionResult GetTasks()
        {
            //var roleClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "Role");
            //var idClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "id");
            //var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "sub");

            List<taskModel> taskList = new List<taskModel>();
            taskList = db.TASKS.Select(task => new taskModel()
            {
                TASKID = task.TASKID,
                TEAM_TASK = task.TEAM_TASK,
                ASSIGNED_TEAM = task.ASSIGNED_TEAM,
                ASSIGNED_USER = task.ASSIGNED_USER,
                CREATED_BY = task.CREATED_BY,
                CREATED_AT = task.CREATED_AT,
                DEADLINE = task.DEADLINE,
                MODIFIED_BY = task.MODIFIED_BY,
                MODIFIED_AT = task.MODIFIED_AT,
                TASK_TITLE = task.TASK_TITLE,
                TASK_DESCRIPTION = task.TASK_DESCRIPTION,
                STATUS = task.STATUS,
                STATUS_COMMENT = task.STATUS_COMMENT,
                RESULT = task.RESULT,
                CLOSED_AT = task.CLOSED_AT
            }).ToList();
            taskList.ForEach(t =>
            {
               if(t.TEAM_TASK == true)
                {
                    if (db.TEAMS.Count(q => q.TEAMID == t.ASSIGNED_TEAM) > 0)
                    {
                        t.ASSIGNED_TEAM_NAME = db.TEAMS.FirstOrDefault(q => q.TEAMID == t.ASSIGNED_TEAM).TEAM_NAME;
                    }
                    
                }
                else
                {
                    if (db.USERS.Count(q => q.USERID == t.ASSIGNED_USER) > 0)
                    {
                        t.ASSIGNED_USER_USERNAME = db.USERS.FirstOrDefault(q => q.USERID == t.ASSIGNED_USER).USERNAME;
                    }

                }


            });

            return Ok(taskList);
        }

        [Authorize(Roles = "admin, depManager, compManager")]
        [HttpGet]
        [Route("api/tasks/{taskId}")]
        public IHttpActionResult GetTask(int taskId)
        {
            if (db.TASKS.Count(q => q.TASKID == taskId) < 1)
            {
                return BadRequest("Belirtilen ID ile kayıt bulunamadı");
            }
            taskModel record = db.TASKS.Where(q => q.TASKID == taskId).Select(task => new taskModel()
            {
                TASKID = task.TASKID,
                TEAM_TASK = task.TEAM_TASK,
                ASSIGNED_TEAM = task.ASSIGNED_TEAM,
                ASSIGNED_USER = task.ASSIGNED_USER,
                CREATED_BY = task.CREATED_BY,
                CREATED_AT = task.CREATED_AT,
                DEADLINE = task.DEADLINE,
                MODIFIED_BY = task.MODIFIED_BY,
                MODIFIED_AT = task.MODIFIED_AT,
                TASK_TITLE = task.TASK_TITLE,
                TASK_DESCRIPTION = task.TASK_DESCRIPTION,
                STATUS = task.STATUS,
                STATUS_COMMENT = task.STATUS_COMMENT,
                RESULT = task.RESULT,
                CLOSED_AT = task.CLOSED_AT
            }).SingleOrDefault();

            return Ok(record);
        }

        //[Authorize(Roles = "admin, depmanager, compManager")]
        [HttpPost]
        [Route("api/tasks")]
        public IHttpActionResult AddTask(TASKS newTask)
        {
            if (db.TASKS.Count(q => q.TASKID == newTask.TASKID) > 0)
            {
                return BadRequest("Sistemde bu id numarası ile başka bir kayıt var!");
            }

            db.TASKS.Add(newTask);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt Başarılı";
            return Ok(resp);
        }

        [Authorize(Roles = "admin, depmanager, compManager")]
        [HttpPut]
        [Route("api/tasks/{taskId}")]
        public IHttpActionResult UpdateTask(int taskId, taskModel task)
        {
            if (db.TASKS.Count(q => q.TASKID == task.TASKID) < 1)
            {
                return BadRequest("Sistemde bu id numarası ile kayıt bulunamadı!");
            }

            var record = db.TASKS.SingleOrDefault(q => q.TASKID == taskId);

            record.TEAM_TASK = task.TEAM_TASK;
            record.ASSIGNED_TEAM = task.ASSIGNED_TEAM;
            record.ASSIGNED_USER = task.ASSIGNED_USER;
            record.CREATED_BY = task.CREATED_BY;
            record.CREATED_AT = task.CREATED_AT;
            record.DEADLINE = task.DEADLINE;
            record.MODIFIED_BY = task.MODIFIED_BY;
            record.MODIFIED_AT = task.MODIFIED_AT;
            record.TASK_TITLE = task.TASK_TITLE;
            record.TASK_DESCRIPTION = task.TASK_DESCRIPTION;
            record.STATUS = task.STATUS;
            record.STATUS_COMMENT = task.STATUS_COMMENT;
            record.RESULT = task.RESULT;
            record.CLOSED_AT = task.CLOSED_AT;
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt başarıyla güncellendi";
            return Ok(resp);
        }

        [Authorize(Roles = "admin, depmanager, compManager")]
        [HttpDelete]
        [Route("api/tasks/{taskId}")]
        public IHttpActionResult DeleteTask(int taskId)
        {
            var record = db.TASKS.SingleOrDefault(q => q.TASKID == taskId);
            db.TASKS.Remove(record);
            db.SaveChanges();
            resp.OK = true;
            resp.message = "Kayıt Silindi.";
            return Ok(resp);
        }

        #region Todos

        [HttpGet]
        [Route("api/todos/{userId}")]
        public IHttpActionResult GetMyTodos(int userId)
        {
            var context = Request.GetOwinContext();
            var userClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.SingleOrDefault(q => q.Type == "sub");
            if (userClaim.Value != db.USERS.FirstOrDefault(q => q.USERID == userId).USERNAME )
            {
                resp.OK = false;
                resp.message = "Kullanıcı doğrulanamadı";
                return BadRequest(resp.message);
            }

            List<todoModel> todos = db.PERSONAL_TASKS.Where(q => q.USERREF == userId && q.STATUS != 3).Select(q => new todoModel() { 
                TASKID = q.TASKID,
                TASK_DESCRIPTION = q.TASK_DESCRIPTION,
                TASK_TITLE = q.TASK_TITLE,
                STATUS = q.STATUS,
                USERREF = q.USERREF
            
            }).ToList(); 
            return Ok(todos);
        }

        [HttpPost]
        [Route("api/todos")]
        public IHttpActionResult AddTodo(PERSONAL_TASKS todo)
        {
            if (todo.TASK_TITLE == null || todo.TASK_TITLE == "" || todo.TASK_DESCRIPTION == null || todo.TASK_DESCRIPTION == "")
            {
                resp.OK = false;
                resp.message = "Eksik Bilgi Girişi";
                return BadRequest(resp.message);
            }
            todo.STATUS = 0;
            db.PERSONAL_TASKS.Add(todo);
            db.SaveChanges();

            resp.OK = true;
            resp.message = "Kayıt Başarılı.";
            return Ok(resp);
        }

        [HttpPut]
        [Route("api/todos/{id}")]
        public IHttpActionResult UpdateTodo(PERSONAL_TASKS todo, int id)
        {
            if (todo.TASK_TITLE == null || todo.TASK_TITLE == "" || todo.TASK_DESCRIPTION == null || todo.TASK_DESCRIPTION == "")
            {
                resp.OK = false;
                resp.message = "Eksik Bilgi Girişi";
                return BadRequest(resp.message);
            }
            var rec = db.PERSONAL_TASKS.FirstOrDefault(q => q.TASKID == id);
            rec.STATUS = todo.STATUS;
            rec.TASK_TITLE = todo.TASK_TITLE;
            rec.TASK_DESCRIPTION = todo.TASK_DESCRIPTION;
            db.SaveChanges();

            resp.OK = true;
            resp.message = "Güncelleme Başarılı.";
            return Ok(resp);
        }

        #endregion



        #endregion

    }
}
