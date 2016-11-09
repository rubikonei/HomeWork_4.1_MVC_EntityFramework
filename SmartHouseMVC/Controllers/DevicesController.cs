using SmartHouseMVC.Models;
using SmartHouseMVC.Models.AbstractDevices;
using SmartHouseMVC.Models.Factories;
using SmartHouseMVC.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;

namespace SmartHouseMVC.Controllers
{
    public class DevicesController : Controller
    {
        private DevicesContext db = new DevicesContext();
        
        // GET: Devices
        public ActionResult Index()
        {
            SelectListItem[] devicesList = new SelectListItem[4];
            devicesList[0] = new SelectListItem { Text = "Кондиционер", Value = "conditioner", Selected = true };
            devicesList[1] = new SelectListItem { Text = "Конвектор", Value = "convector" };
            devicesList[2] = new SelectListItem { Text = "Счетчик электроэнергии", Value = "energyMeter" };
            devicesList[3] = new SelectListItem { Text = "Датчик температуры", Value = "temperatureSensor" };
            ViewBag.DevicesList = devicesList;

            return View(db.Devices);
        }

        public ActionResult Add(string deviceType)
        {
            Device newDevice;
            Factory factory = new Factory();

            switch (deviceType)
            {
                default:
                    newDevice = factory.GetConditioner();
                    break;
                case "convector":
                    newDevice = factory.GetConvector();
                    break;
                case "energyMeter":
                    newDevice = factory.GetEnergyMeter();
                    break;
                case "temperatureSensor":
                    newDevice = factory.GetTemperatureSensor();
                    break;
            }

            db.Devices.Add(newDevice);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Off(Guid id)
        {
            Device device = db.Devices.Find(id);
            if (device.State == true)
            {
                device.Off();
            }
            else
            {
                device.On();
            }
            db.Entry(device).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DecreaseTemperature(Guid id)
        {
            Device device = db.Devices.Find(id);
            ((ClimateDevice)device).Decrease();
            db.Entry(device).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult IncreaseTemperature(Guid id)
        {
            Device device = db.Devices.Find(id);
            ((ClimateDevice)device).Increase();
            db.Entry(device).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SetAutoTemperature(Guid id)
        {
            Device climateDevice = db.Devices.Find(id);
            foreach (Device deviceSensor in db.Devices)
            {
                if (deviceSensor is ITemperatureSensor)
                {
                    ((ClimateDevice)climateDevice).TemperatureEnvironment = ((ITemperatureSensor)deviceSensor).TemperatureEnvironment;
                }
            }
            ((ClimateDevice)climateDevice).SetAutoTemperature();
            db.Entry(climateDevice).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CountEnergy(Guid id)
        {
            int key = 1;
            Device device = db.Devices.Find(id);
            Dictionary<int, Device> allDevices = new Dictionary<int, Device>();
            foreach (Device d in db.Devices)
            {
                allDevices.Add(key++, d);
            }
            ((ICountEnergy)device).CountEnergy(allDevices);
            db.Entry(device).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult OffFan(Guid id)
        {
            Device device = db.Devices.Find(id);
            if (((IFan)device).Fan == true)
            {
                ((IFan)device).FanOff();
            }
            else
            {
                ((IFan)device).FanOn();
            }
            db.Entry(device).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            Device device = db.Devices.Find(id);
            db.Devices.Remove(device);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}