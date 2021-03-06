﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;

namespace TITcs.SharePoint.SSOM.Extensions
{
    public static class SPWebExtensions
    {
        #region SiteUsers
        public static ICollection<User> GetUsers(this SPWeb web)
        {
            return web.SiteUsers.Cast<SPUser>().Select(g => new User() { Id = g.ID.ToString(), Name = g.Name, Login = g.LoginName })
                    .ToList();
        }

        public static ICollection<User> GetUsersByGroup(this SPWeb web, string groupName)
        {
            var group = web.SiteGroups.GetByName(groupName);
            return group.Users.Cast<SPUser>().Select(g => new User() { Id = g.ID.ToString(), Name = g.Name, Login = g.LoginName })
                    .ToArray();

        }

        public static User GetUser(this SPWeb web, string login)
        {
            var currentUser = web.SiteUsers.Cast<SPUser>().FirstOrDefault(i => i.LoginName.Equals(login));
            return BindUser(currentUser);
        }

        public static User GetUserById(this SPWeb web, int id)
        {
            var currentUser = web.SiteUsers.GetByID(id);
            return BindUser(currentUser);
        }

        public static User CurrentUser(this SPWeb web)
        {
            var currentUser = web.CurrentUser;
            return BindUser(currentUser);
        }

        private static User BindUser(SPUser currentUser)
        {
            if (currentUser != null)
                return new User()
                {
                    Claims = currentUser.LoginName,
                    Id = currentUser.ID.ToString(),
                    Login = currentUser.LoginName,
                    Name = currentUser.Name,
                    Groups = currentUser.Groups.Cast<SPGroup>().Select(g => new Group
                    {
                        Id = g.ID.ToString(),
                        Name = g.Name
                    }).ToList()
                };
            return null;
        }
        #endregion SiteUsers

        #region SiteGroups
        public static ICollection<Group> GetGroups(this SPWeb web)
        {
            return web.SiteGroups.Cast<SPGroup>()
                    .Select(g => new Group() { Id = g.ID.ToString(), Name = g.Name })
                    .ToArray();
        }

        public static Group GetGroupByName(this SPWeb web, string name)
        {

            var group = web.SiteGroups.GetByName(name);
            return new Group { Id = group.ID.ToString(), Name = group.Name };


        }
        #endregion SiteGroups

        public static Dictionary<string, string[]> LoadFieldValues(this SPWeb web, params string[] fieldName)
        {
            var dic = new Dictionary<string, string[]>();
            var items = fieldName.Select(f => web.AvailableFields.GetFieldByInternalName(f)).ToArray();
            foreach (var spField in items)
            {
                var choiceField = spField as SPFieldChoice;
                if (choiceField != null)
                    dic.Add(spField.InternalName, choiceField.Choices.Cast<string>().ToArray());
            }
            return dic;
        }
    }
}
