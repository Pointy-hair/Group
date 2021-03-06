﻿using System;
using System.Collections.Generic;
using RevolutionaryStuff.Core;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Traffk.Bal.Permissions;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace TraffkPortal.Models.RoleModels
{
    public class RoleDetailViewModel
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public IList<SelectListItem> InitialPermissionList { get; set; }
        public IEnumerable<string> SelectedPermissions { get; set; }

        public RoleDetailViewModel()
        { }

        public RoleDetailViewModel(ApplicationRole role)
        {
            Id = role.Id;
            RoleName = role.Name;
            var items = new List<SelectListItem>();
            foreach (PermissionNames p in Enum.GetValues(typeof(PermissionNames)))
            {
                items.Add(new SelectListItem { Text = p.ToString().ToTitleFriendlyString(), Value = p.ToString(), Selected = role.HasPermission(p) });
            }
            items.Sort((a, b) => a.Text.CompareTo(b.Text));
            InitialPermissionList = items;
            SelectedPermissions = items.Where(z=>z.Selected).ConvertAll(z => z.Value).ToList();
        }
    }
}
