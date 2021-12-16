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
using System.Collections.ObjectModel;

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
            }
            catch (Exception)
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
            try
            {
                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                String id = "@" + model.Username.Trim();

                var setter = _client.Update("Users" + "/" + id + "/", model);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
            }
            catch (Exception)
            {
                return true;
            }
        }

        public ObservableCollection<string> FindUsers(string name)
        {
            ObservableCollection<string> users = new ObservableCollection<string>();

            IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
            var res = _client.Get("Users");
            var data = JsonConvert.DeserializeObject<Dictionary<string, UserModel>>(res.Body.ToString());
            if (data != null)
            {
                foreach (KeyValuePair<string, UserModel> item in data)
                {
                    if (item.Key.Contains(name))
                    {
                        users.Add(item.Key);
                    }
                }
            }
        return users;
        }
        
        public async Task<ObservableCollection<string>> FindUsers(string name, string owner)
        {
            ObservableCollection<string> users = new ObservableCollection<string>();
            await Task.Run(() =>
            {
                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var res = _client.Get("Users");
                var data = JsonConvert.DeserializeObject<Dictionary<string, UserModel>>(res.Body.ToString());
                if (data != null)
                {
                    foreach (KeyValuePair<string, UserModel> item in data)
                    {
                        if (item.Key.Contains(name) && item.Key != "@" + owner)
                        {
                            users.Add(item.Key);
                        }
                    }
                }
            });
            return users;
        }

        public async Task InviteUser(string user, string from, string titleBoard)
        {
            await Task.Run(() =>
            {
                NotificationsBoard notification = new NotificationsBoard();
                notification.User = user;
                notification.From = from;
                notification.TitleBoard = titleBoard;

                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var res = _client.Get("Users/" + user + "/NotificationsBoard");
                var data = JsonConvert.DeserializeObject<ObservableCollection<NotificationsBoard>>(res.Body.ToString());
                if (data != null)
                {
                    data.Add(notification);
                    var setter = _client.Set("Users" + "/" + user + "/NotificationsBoard/", data);
                }
                else
                {
                    var setter = _client.Set("Users" + "/" + user + "/NotificationsBoard/0", notification);
                }
            });
        }

        public async Task DeleteInivteUser(string user, string from, string titleBoard)
        {
            await Task.Run(() =>
            {
                NotificationsBoard notification = new NotificationsBoard();
                notification.User = user;
                notification.From = from;
                notification.TitleBoard = titleBoard;

                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var res = _client.Get("Users/" + user + "/NotificationsBoard");
                var data = JsonConvert.DeserializeObject<ObservableCollection<NotificationsBoard>>(res.Body.ToString());
                if (data != null)
                {
                    var buff = new NotificationsBoard();
                    foreach(var item in data)
                    {
                        if (item.User == notification.User && item.From == notification.From && item.TitleBoard == notification.TitleBoard)
                        {
                            buff = item;
                            break;
                        }
                    }
                    data.Remove(buff);
                    var setter = _client.Set("Users" + "/" + user + "/NotificationsBoard/", data);
                }
            });
        }

        public async Task<ObservableCollection<NotificationsBoard>> GetNotify(string username)
        {
            ObservableCollection<NotificationsBoard> notify = new ObservableCollection<NotificationsBoard>();
            await Task.Run(() =>
            {
                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var res = _client.Get("Users/" + username + "/NotificationsBoard");
                var data = JsonConvert.DeserializeObject<ObservableCollection<NotificationsBoard>>(res.Body.ToString());
                if (data != null)
                {
                    notify = data;
                }
            });

            return notify;
        }

        public async Task DeletePartFromBoard(string username, string from, string titleboard)
        {
            await Task.Run(() =>
            {
                string buff = "";
                int index = -1;

                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var res = _client.Get("Users/" + from + "/userBoards");
                var data = JsonConvert.DeserializeObject<ObservableCollection<KanbanBoardModel>>(res.Body.ToString());
                if (data != null)
                {
                    for(int i = 0; i < data.Count; i++)
                    {
                        if(data[i].TitleBoard == titleboard)
                        {
                           foreach(var part in data[i].Participants)
                            {
                                if(part == username)
                                {
                                    buff = username;
                                    index = i;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    data[index].Participants.Remove(buff);
                    var setter = _client.Set("Users/" + from + "/userBoards", data);

                }
            });
        }

        public async Task PushMessage(ChatModel message, string ChatId)
        {
            await Task.Run(() =>
            {
                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var res = _client.Get("Chats/" + ChatId);
                var data = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(res.Body.ToString());
                if (data != null)
                {
                    data.Add(message);
                    var setter = _client.Set("Chats/" + ChatId, data);
                }
                else
                {
                    ObservableCollection<ChatModel> newCollectionDB = new ObservableCollection<ChatModel>();
                    newCollectionDB.Add(message);
                    var setter = _client.Set("Chats/" + ChatId, newCollectionDB);
                }
            });
        }

        public async Task<ObservableCollection<ChatModel>> GetMessages(string ChatId)
        {
            ObservableCollection<ChatModel> chatMessages = new ObservableCollection<ChatModel>();
            await Task.Run(() =>
            {
                IFirebaseClient _client = new FireSharp.FirebaseClient(conf.fcon);
                var res = _client.Get("Chats/" + ChatId);
                var data = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(res.Body.ToString());
                if (data != null)
                {
                    chatMessages = data;
                }
            });

            return chatMessages;
        }

        private string GetHashString(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);

            return result.ToLower();
        }

    }
}
