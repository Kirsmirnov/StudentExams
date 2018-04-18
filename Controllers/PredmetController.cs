﻿using StudentExams.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentExams.Controllers
{
    public class PredmetController : Controller
    {
        public ActionResult Index()
        {
            if (!Authentication.IsAdmin && !Authentication.IsTeacher)
                throw new UnauthorizedAccessException();

            ViewBag.Items = GetListItems();
            return View("Index");
        }


        private IList GetListItems()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.Predmet.AsNoTracking()
                    .OrderBy(t => t.Name)
                    .Select(t => new
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList();
            }
        }


        public ActionResult Edit(int? id)
        {
            try
            {
                Predmet model;
                if (id != null)
                {
                    using (DataContext ctx = new DataContext())
                    {
                        model = ctx.Predmet.First(t => t.Id == id);
                    }
                }
                else
                {
                    model = new Predmet();
                    model.InitNewObject();
                }
                return View("Edit", model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Index();
            }
        }


        public ActionResult Save(Predmet model)
        {
            try
            {
                string msg = model.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    ViewBag.Message = msg;
                    return View("Edit", model);
                }
                using (DataContext ctx = new DataContext())
                {
                    if (model.Id != 0)
                    {
                        var item = ctx.Predmet.FirstOrDefault(t => t.Id == model.Id);
                        item.Name = model.Name;
                    }
                    else
                    {
                        ctx.Predmet.Add(model);
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Edit(model.Id);
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {
                using (DataContext ctx = new DataContext())
                {
                    var item = ctx.Predmet.FirstOrDefault(t => t.Id == id);
                    if (item != null)
                    {
                        ctx.Predmet.Remove(item);
                        ctx.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return Index();
        }
    }
}