﻿using FacadeCreatorApi.Forms;
using FacadeCreatorApi.models;
using FacadeCreatorApi.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreatorApi
{
    public class Plugin
    {
        KD.SDK.Appli _appli;

        public Plugin()
        {
            _appli = new KD.SDK.Appli();
        }

        public KD.SDK.Appli CurrentAppli
        {
            get
            {
                return _appli;
            }
        }

        public bool OnPluginLoad(int iCallParamsBlock)
        {
            if (!verifyAccount()) return false;
            KdSdkApiImpl kdApi = new KdSdkApiImpl(iCallParamsBlock);
            //kdApi.updatePalitra();
            AddMenu(iCallParamsBlock);
            String path = StringResources.getResourcesPath()+"\\"+StringResources.getImageDirectoryName();
            //MessageBox.Show(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(path + "\\gray.jpg"))
            {
                ImageConversion.createBackgroundImage(path + "\\gray.jpg");
            }
            return true;
        }
        public bool OnPluginUnload(int lCallParamBlock)
        {
            return true;
        }
        private bool verifyAccount()
        {
            String str = "cp5";
            String strUser = "0000000000";

            long userId = -1;
            long.TryParse(strUser, out userId);
            //MessageBox.Show(strUser);
            if (userId == 0 && !str.Equals("cp6")) return true;
            
                char[] key = new char[] { '9', '4', '2', '0', '8', '1', '7', '5', '3', '6' };
                long salt = 1001001;
                userId = 0;
                StringBuilder builder = new StringBuilder();
                foreach (char item in strUser)
                {
                    builder.Append(key[Int32.Parse(item + "")]);
                }
                long.TryParse(builder.ToString(), out userId);
                userId += salt;
                long currentAccount = 0;
                long.TryParse(_appli.GetAccountNumber(), out currentAccount);
                if (userId == currentAccount) return true;
            return false;
        }
        public bool AddMenu(int iCallParamsBlock)
        {
            
            //System.Security.Cryptography.MD5 crypto = System.Security.Cryptography.MD5.Create();
            //byte[] account = Encoding.UTF8.GetBytes(str);
            //byte[] currentAccount = crypto.ComputeHash(Encoding.UTF8.GetBytes(_appli.GetAccountNumber()));
            //for (int i = 0; i < currentAccount.Length; i++)
            //{
            //    MessageBox.Show(account[i] + "||" + currentAccount[i]);
                
            //}
            //MessageBox.Show(str+"\n"+ Encoding.ASCII.GetString(crypto.ComputeHash(account)));
           // MessageBox.Show();
            //if (!str.Equals(Encoding.ASCII.GetString(crypto.ComputeHash(account)))) return false;
            //if (!str.Equals("")) return false;

            //if (userId != ) return false;
            string MENU_ITEM_TEXT = "Создать изображение на фасаде";
            string MENU_ITEM_PLUGIN_NAME = "FacadeCreatorApi.dll"; // can be an external plugin dll like "KD.Plugin.Tiny.dll" must be declared in SPACE.INI in dll case
                                                                 //string MENU_ITEM_PLUGIN_NAME = "KD.Plugin.Tiny.dll"; // can be an external plugin dll like "KD.Plugin.Tiny.dll" must be declared in SPACE.INI in dll case
            string MENU_ITEM_PLUGIN_METHOD_NAME = "CallMe"; // can be an external plugin dll like "KD.Plugin.Tiny.dll" must be declared in SPACE.INI in dll case
            int menuID = CurrentAppli.InsertMenuItemFromId(MENU_ITEM_TEXT, /* menuItemText */
                                                            KD.SDK.AppliEnum.ControlKey.CK_NONE, /* accelControlKey */
                                                            KD.SDK.AppliEnum.VirtualKeyCode.VirtualKey_NONE, /* accelKeyCode */
                                                            String.Empty, /* iconFileName */
                                                            true, /* InsertBefore */
                                                            (int)KD.SDK.AppliEnum.ObjectMenuItemsId.COMPONENTS, /* menuPosition */
                                                            MENU_ITEM_PLUGIN_NAME, /* pluginDllFileName, */
                                                            "Plugin", /* className */
                                                            MENU_ITEM_PLUGIN_METHOD_NAME/* functionName */);

            if (menuID <= 0)
            {
                return false;
            }

           // System.Windows.Forms.MessageBox.Show("InsertMenuItemFromId was succesful." + Environment.NewLine + "Check for menu item Place | \"" + MENU_ITEM_TEXT + "\"" + Environment.NewLine + "Method " + MENU_ITEM_PLUGIN_METHOD_NAME + "() must exist in " + MENU_ITEM_PLUGIN_NAME);

            return true;
        }

        public bool CallMe(int iCallParamsBlock)
        {
            MainForm frm = new MainForm();    

            KdSdkApi kdApi = new KdSdkApiImpl(iCallParamsBlock);
            Scenes scenes = new Scenes(frm,kdApi);
            ICollection<FigureOnBoard> facades;
            try
            {
                //MessageBox.Show("Before geting facades");
                facades = kdApi.getFacades();
                //MessageBox.Show("After geting facades");
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show("Объекты с атрибутом по свойству 1 не выбраны");
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Something wrong in getting facades \n "+e.Message);
                return false;
            }

                foreach (FigureOnBoard item in facades)
            {
                if (item.figure.width == 0 || item.figure.height == 0)
                {
                    MessageBox.Show("Element with id:" + item.figure + " have one or more zero parameters");
                }
                else
                {
                    if (((Facade)item.figure).getTextureId() != -1)
                    {
                        string path = kdApi.getImagePathFromTexture(((Facade)item.figure).getTextureId());
                        //MessageBox.Show(path);
                        if (!path.Equals(""))
                        {
                            Bitmap image = ImageConversion.getImage(path);
                            scenes.addFigure(new BkgImage(image), item.x, item.y);
                        }
                    }
                    scenes.addFigure(item.figure, item.x, item.y);
                }
            }
            //scenes.addFigure(new Facade(21, 100, 100), 0, 0);
            if (facades.Count < 1)
            {
                MessageBox.Show("Объекты с атрибутом по свойству 1 не выбраны");
                return false;
            }       
            frm.ShowDialog();
            
            return true;
        }
    }
}
