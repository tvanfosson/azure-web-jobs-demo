﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using WebApp.Models;
using WebApp.Services;
using WebJobDemo.Core.Data;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionQuery _subscriptionQuery;
        private readonly IMapper _mapper;

        public HomeController(ISubscriptionService subscriptionService, ISubscriptionQuery subscriptionQuery, IMapper mapper)
        {
            _subscriptionService = subscriptionService;
            _subscriptionQuery = subscriptionQuery;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Subscribe()
        {
            return View(new SubscriberViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Subscribe(SubscriberViewModel subscriber)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var subscription = await _subscriptionService.SignUp(subscriber.FirstName, subscriber.LastName, subscriber.EmailAddress);

                    var model = _mapper.Map<SubscriptionViewModel>(subscription);

                    return View("Subscribed", model);
                }
                catch (Exception e)
                {
                    if (e.Message.StartsWith("Cannot insert duplicate", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("EmailAddress", "Address already in use. Please try another.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Something bad happened. Please try again later.");
                    }
                }
            }

            return View(subscriber);
        }

        public async Task<ActionResult> Subscribed(SubscriberLookupModel lookup)
        {
            var model = SubscriptionViewModel.Convert(lookup);

            if (ModelState.IsValid)
            {
                Contract.Assume(lookup.SubscriptionKey.HasValue);

                var subscription = await _subscriptionQuery.GetSubscriptionByEmailAddressAndSubscriptionKey(lookup.EmailAddress, lookup.SubscriptionKey.Value);

                if (subscription != null)
                {
                    model = _mapper.Map<SubscriptionViewModel>(subscription);
                }

                model.PerformedLookup = true;
            }

            return View(model);
        }

        public async Task<ActionResult> Confirm(SubscriberLookupModel lookup)
        {
            var model = SubscriptionViewModel.Convert(lookup);

            if (ModelState.IsValid)
            {
                Contract.Assume(lookup.SubscriptionKey.HasValue);

                var subscription = await _subscriptionQuery.GetSubscriptionByEmailAddressAndSubscriptionKey(lookup.EmailAddress, lookup.SubscriptionKey.Value);

                model.PerformedLookup = true;

                if (subscription != null)
                {
                    try
                    {
                        await _subscriptionService.Confirm(subscription);
                        subscription.Confirmed = true;
                    }
                    catch (Exception)
                    {
                        // TODO: log this.... :)
                    }

                    model = _mapper.Map<SubscriptionViewModel>(subscription);
                }
            }

            return View("Subscribed", model);
        }

        public async Task<ActionResult> Statistics()
        {
            var stats = await _subscriptionQuery.GetStatistics();

            var model = _mapper.Map<ICollection<DomainStatisticsViewModel>>(stats);

            return View(model);
        }
    }
}