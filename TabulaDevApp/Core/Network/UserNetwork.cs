using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TabulaDevApp.MVVM.Models;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using Newtonsoft.Json;

namespace TabulaDevApp.Core.Network
{
    public class UserNetwork
    {
        Configuration conf;
        public UserNetwork()
        {
            conf = new Configuration();
        }

        public UserModel Entrance(AuthorizationModel model)
        {
            try
            {
                UserModel user = null;
                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var outer = Task.Factory.StartNew(() =>      // внешняя задача
                {
                    var res = _client.Get("Users");
                    var inner = Task.Factory.StartNew(() =>  // вложенная задача
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, UserModel>>(res.Body.ToString());
                        if (data != null)
                        {
                            foreach (KeyValuePair<string, UserModel> item in data)
                            {
                                if (item.Value.Email == model.Login && item.Value.Password == GetHashString(model.Password))
                                {
                                    user = item.Value;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            user = null;
                        }

                    }, TaskCreationOptions.AttachedToParent);
                });

                outer.Wait(); // ожидаем выполнения внешней задачи
                return user;
            } catch (Exception)
            {
                return null;
            }
        }

        public UserModel CreateUser(RegistrationModel model)
        {
            UserModel user = new UserModel();
            IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
            String id = "@" + model.Username.Trim();

            user.Email = model.Email;
            user.Username = model.Username;
            user.Password = GetHashString(model.Password);

            var setter = _client.Set("Users" + "/" + id + "/", user);

            return user;
        }

        public bool UpdateUser(UserModel model)
        {
            //try
            //{
                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                String id = "@" + model.Username.Trim();

                var setter = _client.Update("Users" + "/" + id + "/", model);

                return true;
            //}
            //catch (Exception e)
            //{
            //    return false;
            //}
        }

        public bool FreeNicknameCheck(string nickname)
        {
            try
            {
                bool result = true;
                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var outer = Task.Factory.StartNew(() =>      // внешняя задача
                {
                    var res = _client.Get("Users");
                    var inner = Task.Factory.StartNew(() =>  // вложенная задача
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, UserModel>>(res.Body.ToString());
                        if (data != null)
                        {
                            foreach (KeyValuePair<string, UserModel> item in data)
                            {
                                if (("@" + nickname.Trim()) == item.Key)
                                {
                                    result = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }, TaskCreationOptions.AttachedToParent);
                });

                outer.Wait(); // ожидаем выполнения внешней задачи

                return result;
            }catch (Exception)
            {
                return true;
            }
        }

        private string GetHashString(string s)
        {
            //переводим строку в байт-массим  
            byte[] bytes = Encoding.Unicode.GetBytes(s);

            //создаем объект для получения средст шифрования  
            MD5CryptoServiceProvider CSP =
                new MD5CryptoServiceProvider();

            //вычисляем хеш-представление в байтах  
            byte[] byteHash = CSP.ComputeHash(bytes);

            string hash = string.Empty;

            //формируем одну цельную строку из массива  
            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return hash;
        }
    }
}
