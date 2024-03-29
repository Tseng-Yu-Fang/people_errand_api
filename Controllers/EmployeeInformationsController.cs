﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using People_errand_api.Models;

namespace People_errand_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeInformationsController : ControllerBase
    {
        private readonly people_errandContext _context;

        public EmployeeInformationsController(people_errandContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeInformations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeInformation>>> GetEmployeeInformations()
        {
            return await _context.EmployeeInformations.ToListAsync();
        }


        //// GET: api/EmployeeInformations/hash_account
        //[HttpGet("{hash_account}")]
        //public IEnumerable<Employee> GetEmployeeInformation(string hash_account)
        //{

        //    List<Employee> employee;

        //    var parameters = new[]
        //    {
        //        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
        //        {
        //            Direction = System.Data.ParameterDirection.Input,
        //            Value = hash_account,
        //        }
        //    };

        //    employee = _context.Employees.FromSqlRaw("exec get_employeeInformaion @hash_account", parameters).ToList();

        //    return employee;
        //}


        [HttpGet("{hash_account}")]
        public async Task<IEnumerable> GetEmployeeInformation(string hash_account)
        {

            var Employee_information = await (from t in _context.Employees
                                              join a in _context.EmployeeInformations on t.HashAccount equals a.HashAccount
                                              join b in _context.EmployeeDepartmentTypes on a.DepartmentId equals b.DepartmentId
                                              join c in _context.EmployeeJobtitleTypes on a.JobtitleId equals c.JobtitleId
                                              where t.HashAccount == hash_account
                                              select new
                                              {
                                                  name = a.Name,
                                                  department = b.Name,
                                                  jobtitle = c.Name,
                                                  phone = a.Phone,
                                                  email = a.Email,
                                              }).ToListAsync();
            return Employee_information;
        }

        [HttpGet("BoolEmployeeInformationEmail")]
        public async Task<bool> BoolEmployeeInformationEmail(string hash_company,string email)
        {

            var Employee_information = await (from t in _context.EmployeeInformations
                                              join a in _context.Employees on t.HashAccount equals a.HashAccount
                                              where a.CompanyHash==hash_company && t.Email == email
                                              select new
                                              {
                                                  CompanyHash=a.CompanyHash,
                                                  Email=t.Email
                                              }).ToListAsync();

            bool result = Employee_information.Count() != 0 ? true : false;

            return result;
        }


        // PUT: api/update_information
        [HttpPut("update_information")]//APP用修改員工資料
        public ActionResult<bool> update_information([FromBody] List<EmployeeInformation> employeeInformations)
        {
            bool result = true;
            try
            {
                foreach (EmployeeInformation employeeInformation in employeeInformations)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.HashAccount
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Name
                        },
                        new SqlParameter("@phone",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Phone
                        },
                        new SqlParameter("@email",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Email
                        },
                        new SqlParameter("@img",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Img
                        }
                    };

                    result = _context.Database.ExecuteSqlRaw("exec update_information @hash_account,@name,@phone,@email,@img", parameters: parameters) != 0 ? true : false;
                }

            }
            catch (Exception)
            {
                result = false;
                throw;
            }

            return result;
        }

        public class EditEmployee
        {
            public string HashAccount { get; set; }//員工編號
            public string ManagerHash { get; set; }//管理員編號
            public string Name { get; set; }//員工姓名
            public string Phone { get; set; }//員工電話
            public string Email { get; set; }//員工電子郵件
            public int DepartmentId { get; set; }//員工部門代號
            public int JobTitleId { get; set; }//員工職稱代號
        }//已審核員工資料編輯
        // PUT: api/edit_information
        [HttpPut("edit_information")]//後台用修改員工資料
        public ActionResult<bool> edit_information([FromBody] List<EditEmployee> employeeInformations)
        {
            bool result = true;
            try
            {
                foreach (EditEmployee employeeInformation in employeeInformations)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.HashAccount
                        },
                        new SqlParameter("@manager_hash",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.ManagerHash
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Name
                        },
                        new SqlParameter("@department_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.DepartmentId
                        },
                        new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.JobTitleId
                        },
                        new SqlParameter("@phone",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Phone
                        },
                        new SqlParameter("@email",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Email
                        }
                        
                    };

                    result = _context.Database.ExecuteSqlRaw("exec edit_information @hash_account, @manager_hash,@name,@department_id,@jobtitle_id,@phone,@email", parameters: parameters) != 0 ? true : false;
                }

            }
            catch (Exception)
            {
                result = false;
                throw;
            }

            return result;
        }
        public class SetEmployee
        {
            public string HashAccount { get; set; }//員工編號
            public string ManagerHash { get; set; }//管理員編號
            public int DepartmentId { get; set; }//員工部門
            public int JobtitleId { get; set; }//員工職稱
        }//賦予職稱及部門
        // PUT: api/add_information
        [HttpPut("set_information")]//賦予員工職位及部門
        public ActionResult<bool> set_information([FromBody] List<SetEmployee> employeeInformations)
        {
            bool result = true;
            try
            {
                foreach (SetEmployee employeeInformation in employeeInformations)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.HashAccount
                        },
                        new SqlParameter("@manager_hash",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.ManagerHash
                        },
                        new SqlParameter("@department_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.DepartmentId
                        },
                        new SqlParameter("@jobtitle_id",System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.JobtitleId
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec set_information @hash_account,@manager_hash,@department_id,@jobtitle_id", parameters: parameters) != 0 ? true : false;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        // POST: api/add_information
        [HttpPost("add_information")]//新增員工資料
        public ActionResult<bool> add_information([FromBody]List<EmployeeInformation> employeeInformations)
        {
            bool result = true;
            try
            {
                foreach (EmployeeInformation employeeInformation in employeeInformations)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.HashAccount
                        },
                        new SqlParameter("@name",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Name
                        },
                        new SqlParameter("@email",System.Data.SqlDbType.NVarChar)
                        {
                            Direction = System.Data.ParameterDirection.Input,
                            Value = employeeInformation.Email
                        }
                    };
                    result = _context.Database.ExecuteSqlRaw("exec add_information @hash_account,@name,@email",parameters:parameters) != 0 ? true : false;
                }
            }
            catch (Exception) {
                result = false;
                throw;
            }
            return result;
        }
        // DELETE: api/EmployeeInformations/DeleteInformation/5
        [HttpDelete("DeleteInformation/{hash_account}")]
        public async Task<bool> DeleteInformation(string hash_account)
        {
            bool result = true;
            try
            {
                    var parameters = new[]
                    {
                            new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = hash_account
                            }
                        };
                    result = _context.Database.ExecuteSqlRaw("exec delete_employee @hash_account", parameters: parameters) != 0 ? true : false;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

        //// DELETE: api/EmployeeInformations/5
        //[HttpDelete("delete_information")]
        //public ActionResult<bool> delete_information([FromBody] List<EmployeeInformation> employeeInformations)
        //{
        //    bool result = true;
        //    try
        //    {
        //        foreach (EmployeeInformation employeeInformation in employeeInformations)
        //        {
        //            var parameters = new[]
        //            {
        //                new SqlParameter("@hash_account",System.Data.SqlDbType.VarChar)
        //                {
        //                    Direction = System.Data.ParameterDirection.Input,
        //                    Value = employeeInformation.HashAccount
        //                }
        //            };
        //            result = _context.Database.ExecuteSqlRaw("exec delete_employee @hash_account", parameters: parameters) != 0 ? true : false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = false;
        //        throw;
        //    }
        //    return result;
        //}

        private bool EmployeeInformationExists(int id)
        {
            return _context.EmployeeInformations.Any(e => e.InformationId == id);
        }
    }
}
