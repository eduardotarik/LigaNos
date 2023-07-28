﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LigaNOS.Data;
using LigaNOS.Data.Entities;
using System.Xml.Schema;
using LigaNOS.Helpers;

namespace LigaNOS.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserHelper _userHelper;

        public TeamsController(ITeamRepository teamRepository,
            IUserHelper userHelper)
        {
            _teamRepository = teamRepository;
            _userHelper = userHelper;
        }

        // GET: Teams
        public IActionResult Index()
        {
            return View(_teamRepository.GetAll().OrderBy(t => t.Name));
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _teamRepository.GetByIdAsync(id.Value);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate team name
                var existingTeam = await _teamRepository.GetByNameAsync(team.Name);
                if (existingTeam != null)
                {
                    ModelState.AddModelError("Name", "A team with the same name already exists.");
                    return View(team);
                }

                //TODO: Change to the user that is logged
                team.User = await _userHelper.GetUserByEmailAsync("eduardo@gmail.com");

                await _teamRepository.CreateAsync(team);
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _teamRepository.GetByIdAsync(id.Value);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Team team)
        {
            //if (id != team.Id)
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check for duplicate team name
                var existingTeam = await _teamRepository.GetByNameAsync(team.Name);
                if (existingTeam != null && existingTeam.Id != team.Id)
                {
                    ModelState.AddModelError("Name", "A team with the same name already exists.");
                    return View(team);
                }


                try
                {
                    //TODO: Change to the user that is logged
                    team.User = await _userHelper.GetUserByEmailAsync("eduardo@gmail.com");
                    await _teamRepository.UpdateAsync(team);
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (! await _teamRepository.ExistAsync(team.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _teamRepository.GetByIdAsync(id.Value);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            await _teamRepository.DeleteAsync(team);
            return RedirectToAction(nameof(Index));
        }

    }
}