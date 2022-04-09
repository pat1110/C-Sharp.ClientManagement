using Client_Management.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Client_Management.Services
{
    class DatabaseService
    {
        /// <summary>
        /// Dient dazu die Verbindung zur Datenbank zu prüfen und die DB und das Schema anzulegen, falls nicht vorhanden.
        /// </summary>
        /// <returns>true (bool), wenn erfolgreich</returns>
        /// <exception cref="Exception">Wird geworfen, wenn DB und Schema nicht überprüft/angelegt werden kann.</exception>
        public static async Task<bool> TestConnectionAsync()
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                try
                {
                    await context.Database.EnsureCreatedAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong when connecting to the SQL-Server.\nThe Server could not be reached and/or the given credentials/privileges are not valid.\n\n" + ex.Message);
                }
            }
        }


        #region Computer

        /// <summary>
        /// Ruft alle Computer von der Datenbank ab.
        /// </summary>
        /// <returns>List von Computer</returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task<List<Computer>> GetComputersAsync()
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                try
                {
                    context.Database.EnsureCreated();
                    return await Task.Run(() => context.Computers.Include(computer => computer.Products).Include(computer => computer.QueryUser).ToList());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Fügt neuen Computer in die Datenbank ein oder aktualisiert einen bestehenden Computer in der Datenbank.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task AddComputerAsync(Computer computer)
        {
            using(ClientManagementContext context = new ClientManagementContext())
            {
                try
                {
                    context.Database.EnsureCreated();
                    await Task.Run(() => context.Computers.Update(computer));
                    await Task.Run(() => context.SaveChanges());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// Entfernt den Computer aus der Datenbank.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task RemoveComputerAsync(Computer computer)
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                context.Database.EnsureCreated();
                try
                {
                    await Task.Run(() => context.Remove(computer));
                    await Task.Run(() => context.SaveChanges());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                }
            }
        }

        #endregion

        #region Credentials

        /// <summary>
        /// Ruft alle Credentials von der Datenbank ab.
        /// </summary>
        /// <returns>List von Credential</returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        static public async Task<List<Credential>> GetCredentialsAsync()
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                try
                {
                    context.Database.EnsureCreated();
                    List<Credential> l = await Task.Run(() => context.Credentials.ToList());
                    return l;
                    //return await Task.Run(() => context.Credentials.ToList());
                }catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Fügt eine neue Credential in die Datenbank ein oder aktualisiert einen bestehenden Eintrag.
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task AddCredentialAsync(Credential credential)
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                context.Database.EnsureCreated();
                try
                {
                    await Task.Run(() => context.Credentials.Update(credential));
                    await Task.Run(() => context.SaveChanges());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Entfern einen Credential-Eintrag aus der Datenbank.
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task RemoveCredentialAsync(Credential credential)
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                await Task.Run( () => {
                    try
                    {
                        context.Database.EnsureCreated();

                        context.Remove(context.Credentials.Find(credential.Id));
                        context.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null)
                            throw new Exception("SQL Exception:\n\nPerhaps this credential is still configured for at least one computer.\n\nDetail:\n" + ex.InnerException.Message);
                    }catch (Exception ex)
                    {
                        throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                    }
                });
            }
        }

        #endregion

        #region Groups

        /// <summary>
        /// Fügt eine neue Group in die Datenbank ein oder aktualisiert einen bestehenden Eintrag.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task AddGroupAsync(Group group)
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                await Task.Run( () =>
                {
                    try
                    {
                        context.Database.EnsureCreated();

                        context.Groups.Update(group);
                        context.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null)
                           throw new Exception("SQL Exception:\n\n" + ex.InnerException.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Exception:\n\n" + ex.InnerException?.Message + "\n\n" + ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// Entfernt die Group aus der Datenbank.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task RemoveGroupAsync(Group group)
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                await context.Database.EnsureCreatedAsync();
                /*
                await Task.Run(() =>
                {
                    Parallel.ForEach(group.Parameter, (currentParameter) =>
                    {
                        try
                        {
                            context.Remove(context.GroupParameters.Find(currentParameter.Id));
                        }
                        catch (DbUpdateException ex)
                        {
                            if (ex.InnerException != null)
                                MessageBox.Show("SQL Exception:\n\n" + ex.InnerException.Message, "Error!",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Exception:\n\n" + ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
                });*/
                await Task.Run(() =>
                {
                    try
                    {
                        //context.Remove(context.Groups.Find(group.Id));
                        context.Remove((group));
                        context.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null)
                            throw new Exception("SQL Exception:\n\n" + ex.InnerException.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Exception:\n\n" + ex.InnerException?.Message + "\n\n" + ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// Ruft alle Groups von der Datenbank ab.
        /// </summary>
        /// <returns>List von Group</returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        static public async Task<List<Group>> GetGroupsAsync()
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                try
                {
                    context.Database.EnsureCreated();
                    return await Task.Run(() => context.Groups.Include(group => group.Parameter).ToList());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Ruft alle Computer von der Datenbank ab, die mit dem Filter (groupParameters) übereinstimmen.
        /// </summary>
        /// <param name="groupParameters"></param>
        /// <returns>List von Computer</returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        static public async Task<List<Computer>> GetFilteredComputersAsync(List<GroupParameter> groupParameters)
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                try
                {
                    context.Database.EnsureCreated();
                    IQueryable<Computer> l = await Task.Run(() => context.Computers.Include(computer => computer.QueryUser));

                    foreach (GroupParameter p in groupParameters)
                    {
                        if (p.Condition == null || p.Property == null || p.Search == "" || p.Connection == null)
                            throw new Exception("None of the Filter-Variables may by null (Condition, Connection, Property, Search)");

                        //l = l.Where(p.Property + " = @0", p.Search);

                        if (p.Property.Contains("Product."))
                        {
                            if (p.Condition == "LIKE")
                            {
                                l = l.Where("Products.Any(" + p.Property.Replace("Product.", "") + ".Contains(@0))", p.Search);
                            }
                            else if (p.Condition == "NOT LIKE")
                            {
                                l = l.Where("! Products.Any(" + p.Property.Replace("Product.", "") + ".Contains(@0))", p.Search);
                            }
                            else if (p.Condition == "IS")
                            {
                                l = l.Where("Products.Any(" + p.Property.Replace("Product.", "") + " = @0)", p.Search);
                            }
                            else if (p.Condition == "IS NOT")
                            {
                                l = l.Where("! Products.Any(" + p.Property.Replace("Product.", "") + " = @0)", p.Search);
                            }
                            //IQueryable<Product> lTmp = l1.Where("Name.Contains(@0)", "a");
                            //l.Where("Products.Contains(@0)", lTmp);
                            //l = l.Where("Products.Any(Name.Contains(@0))", "a");
                            //l = l.Where("Products.Any(" + p.Property.Replace("Product.", "") + ".Contains(@0))", p.Search);
                        }
                        else if (p.Condition == "LIKE")
                        {
                            l = l.Where(p.Property + ".Contains(@0)", p.Search);
                        }
                        else if (p.Condition == "NOT LIKE")
                        {
                            l = l.Where("!" + p.Property + ".Contains(@0)", p.Search);
                        }
                        else if (p.Condition == "IS")
                        {
                            l = l.Where(p.Property + " = @0", p.Search);
                        }
                        else if (p.Condition == "IS NOT")
                        {
                            l = l.Where("!" + p.Property + " = @0", p.Search);
                        }
                    }

                    return l.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                }
                //return new List<Computer>(context.Computers.Where("Id = @0 and Ip = @1", 1, "127.0.0.1"));
            }

            /*
                var query =
                db.Customers.Where("City == @0 and Orders.Count >= @1", "London", 10).
                OrderBy("CompanyName").
                Select("New(CompanyName as Name, Phone)");
            */
        }

        /* 
        CREATE VIEW TestView  
        AS  
        SELECT * FROM Computers WHERE Name like "Test";


        SELECT TABLE_NAME, VIEW_DEFINITION FROM information_schema.VIEWS WHERE TABLE_SCHEMA = "ClientManagement";
        */

        #endregion


        #region Jobs

        /// <summary>
        /// Fügt den Job in die Datenbank ein oder aktualisiert einen bestehenden Eintrag.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task AddJobAsync(Job job)
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                context.Database.EnsureCreated();
                try
                {
                    await Task.Run(() => context.Jobs.Update(job));
                    await Task.Run(() => context.SaveChanges());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Entfernt den Job aus der Datenbank.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        static public async Task RemoveJobAsync(Job job)
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                context.Database.EnsureCreated();
                try
                {

                    await Task.Run(() => context.Remove(context.Jobs.Find(job.Id)));
                    await Task.Run(() => context.SaveChanges());
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception:\n\n" + ex.InnerException?.Message + "\n\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Ruft alle Jobs von der Datenbank ab.
        /// </summary>
        /// <returns>List von Job</returns>
        /// <exception cref="Exception">Wird geworfen, wenn der Zugriff auf die Datenbank fehlschlägt.</exception>
        public static async Task<List<Job>> GetJobsAsync()
        {
            using (ClientManagementContext context = new ClientManagementContext())
            {
                context.Database.EnsureCreated();
                try
                {
                    return await Task.Run(() => context.Jobs.ToList());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message + "\n\n" + ex.Message);
                }
            }
        }

        #endregion

    }
}
