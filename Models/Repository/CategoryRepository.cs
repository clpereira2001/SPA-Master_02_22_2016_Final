using System;
using System.Collections.Generic;
using System.Linq;
using Vauction.Utils;
using System.Text;
using Vauction.Models.CustomClasses.JsTree;
using Vauction.Utils.Perfomance;
using Vauction.Models.Enums;
using System.Web;

namespace Vauction.Models
{
    public class CategoryRepository : ICategoryRepository
    {
        #region init
        private VauctionDataContext dataContext;
        private ICacheDataProvider CacheRepository;

        public CategoryRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
        {
            this.dataContext = dataContext;
            CacheRepository = cacheDataProvider;
        }
        #endregion

        #region Get category tree
        //GetCategoriesMapTree
        private void GetCategoriesMapTreeChild(StringBuilder sb, JsTreeNode js, Event evnt, bool demo, CategoryChildLink objCategoryChildLink = null, CategoryChild objCategoryChild = null)
        {
            if (evnt.IsClickable || demo || evnt.IsAccessable)
            {

                objCategoryChildLink.ChildLinkUrl = js.data.attributes.href;
                objCategoryChildLink.ChildLinkName = js.data.title.ToUpper();
                objCategoryChild.ChildLinkUrl = js.data.attributes.href;
                objCategoryChild.ChildLinkName = js.data.title.ToUpper();

                //sb.AppendFormat("<li ischild='{2}' ><a style='{4}' href='{0}'>{1}</a> {3}", js.data.attributes.href, (js.data.attributes.rel != "main") ? js.data.title : js.data.title.ToUpper(), js.data.attributes.rel != "main", (!String.IsNullOrEmpty(js.data.icon)) ? "(" + js.data.icon + ")" : String.Empty, (js.data.attributes.rel != "main") ? " font-weight:normal;color:#0072B0;" : "font-weight:bold;color:gray");
                sb.AppendFormat("<li ischild='{2}' ><a style='{4}' href='{0}'>{1}</a> {3}", js.data.attributes.href, (js.data.attributes.rel != "main") ? js.data.title : js.data.title.ToUpper(), js.data.attributes.rel != "main", (!String.IsNullOrEmpty(js.data.icon)) ? "(" + js.data.icon + ")" : String.Empty, (js.data.attributes.rel != "main") ? " color:gray" : "font-weight:bold;color:gray");
            }
            else
            {
                objCategoryChildLink.ChildLinkUrl = js.data.attributes.rel;
                objCategoryChildLink.ChildLinkName = js.data.title.ToUpper();
                objCategoryChild.ChildLinkUrl = js.data.attributes.rel;
                objCategoryChild.ChildLinkName = js.data.title.ToUpper();
                // sb.AppendFormat("<li ischild='{1}' ><span style='{3}'>{0}</span> {2}", (js.data.attributes.rel != "main") ? js.data.title : js.data.title.ToUpper(), js.data.attributes.rel != "main", (!String.IsNullOrEmpty(js.data.icon)) ? "(" + js.data.icon + ")" : String.Empty, (js.data.attributes.rel != "main") ? " font-weight:normal;color:#0072B0;" : "font-weight:bold;color:gray");
                sb.AppendFormat("<li ischild='{1}' ><span style='{3}'>{0}</span> {2}", (js.data.attributes.rel != "main") ? js.data.title : js.data.title.ToUpper(), js.data.attributes.rel != "main", (!String.IsNullOrEmpty(js.data.icon)) ? "(" + js.data.icon + ")" : String.Empty, (js.data.attributes.rel != "main") ? " color:gray" : "font-weight:bold;color:gray");
            }

            if (js.children != null && js.children.Count() > 0)
            {
                sb.Append("<ul>");
                foreach (JsTreeNode j in js.children)
                    GetCategoriesMapTreeChild(sb, j, evnt, demo);
                sb.Append("</ul>");
            }
            sb.AppendFormat("</li>");
        }

        // GetCategoriesMapTree
        private object GetCategoriesMapTree(long event_id, bool demo, bool isaccessable)
        {

            List<CategoryChildLink> objCategoryChildLinkParent = new List<CategoryChildLink>();
            CategoryParentChild obj = new CategoryParentChild();
            List<CategoryChild> obj1 = new List<CategoryChild>();

            
            DataCacheObject dco = (!demo) ? new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORIESMAPTREE", new object[] { event_id, isaccessable }, CachingExpirationTime.Days_01) : new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORIESMAPTREEPREVIEW", new object[] { event_id }, CachingExpirationTime.Days_01);
            object result = CacheRepository.Get(dco);

            if (result != null) return result; //new { html = result };
            List<JsTreeNode> nodesList = new List<JsTreeNode>();

            JsTreeNode node, nodeParent;

            //event_id = 313;

            var map = (from EC in dataContext.EventCategories
                       where EC.Event_ID == event_id && EC.IsActive && EC.CategoriesMap.Category.IsActive //&& EC.Auctions.Count() > 0
                       orderby ((EC.CategoriesMap.Parent_ID.HasValue) ? EC.CategoriesMap.CategoriesMap_Parent.Category.Title : EC.CategoriesMap.Category.Title) ascending, EC.CategoriesMap.Category.Title ascending
                       select new { CategoryMap = EC.CategoriesMap, AuctionCount = EC.Auctions.Count(), EventCategory = EC }).ToList();
            Dictionary<long, JsTreeNode> tree = new Dictionary<long, JsTreeNode>();
            CategoriesMap tmp;
            bool alreadyInTree;
            foreach (var c in map)
            {
                if (c.AuctionCount == 0 && c.CategoryMap.CategoriesMap_Children.Count() == 0) continue;
                tmp = c.CategoryMap;
                alreadyInTree = tree.ContainsKey(tmp.ID);
                node = (alreadyInTree) ? tree[tmp.ID] : new JsTreeNode();
                node.attributes = new Attributes();
                node.attributes.id = tmp.ID.ToString();
                node.data = new Data();
                node.data.title = tmp.Category.Title;
                node.data.icon = String.Empty;
                node.data.attributes = new Attributes();
                //AppHelper.GetSiteUrl() + 
                node.data.attributes.href = "/" + ((demo) ? "Preview" : "Auction") + "/EventCategory/" + c.EventCategory.ID.ToString() + "/" + UrlParser.TitleToUrl(c.EventCategory.FullCategory);
                node.data.attributes.rel = "main";
                if (alreadyInTree) continue;
                tree.Add(tmp.ID, node);
                while (tmp.Parent_ID.HasValue)
                {
                    tmp = tmp.CategoriesMap_Parent;
                    alreadyInTree = tree.ContainsKey(tmp.ID);
                    nodeParent = (alreadyInTree) ? tree[tmp.ID] : new JsTreeNode();
                    nodeParent.attributes = new Attributes();
                    nodeParent.attributes.id = tmp.ID.ToString();
                    nodeParent.data = new Data();
                    nodeParent.data.title = tmp.Category.Title;
                    nodeParent.data.attributes = new Attributes();
                    //AppHelper.GetSiteUrl() + 
                    nodeParent.data.attributes.href = "/" + ((demo) ? "Preview" : "Auction") + "/Category/" + tmp.ID.ToString() + "/" + UrlParser.TitleToUrl(tmp.Category.Title) + "?e=" + event_id.ToString();
                    node.data.attributes.rel = "child";
                    nodeParent.data.attributes.rel = "main";
                    nodeParent.state = "open";
                    if (nodeParent.children == null) nodeParent.children = new List<JsTreeNode>();
                    nodeParent.children.Add(node);
                    if (alreadyInTree) break;
                    node = nodeParent;
                    tree.Add(tmp.ID, nodeParent);
                }
                if (alreadyInTree) continue;
                nodesList.Add(node);
            }
            //return nodesList;
            StringBuilder sb = new StringBuilder();
            Event evnt = dataContext.Events.SingleOrDefault(E => E.ID == event_id);

            CategoryChild objA1 = null;
            List<CategoryParentChild> lstCategoryParentChild = new List<CategoryParentChild>();
            sb.Append("<ul class=\"list-unstyled\">");
            foreach (JsTreeNode jstree in nodesList)
            {
                CategoryParentChild objA = new CategoryParentChild();

                CategoryChildLink objCategoryChildLink = new CategoryChildLink();
                List<CategoryChildLink> objCategoryChildLinkAll = new List<CategoryChildLink>();
                List<CategoryChildLink> objCategoryChildLink1 = new List<CategoryChildLink>();


                
                int uji = objA.Childs.Count;
                if (evnt.IsClickable || demo || evnt.IsAccessable)
                {
                    objCategoryChildLink.parentLinkUrl = jstree.data.attributes.href;
                    objCategoryChildLink.parentLinkName = jstree.data.title.ToUpper();
                    objA.parentLinkUrl = jstree.data.attributes.href;
                    objA.parentLinkName = jstree.data.title.ToUpper();
                    sb.AppendFormat("<li ischild='{2}' class=\"title-category\" ><a href='{0}'>{1}</a> {3}", jstree.data.attributes.href, (jstree.data.attributes.rel != "main") ? jstree.data.title : jstree.data.title.ToUpper(), jstree.data.attributes.rel != "main", (!String.IsNullOrEmpty(jstree.data.icon)) ? "(" + jstree.data.icon + ")" : String.Empty, (jstree.data.attributes.rel != "main") ? " font-weight:normal;color:#0072B0;" : "font-weight:bold;color:black");
                }
                //  sb.AppendFormat("<li ischild='{2}' class=\"title-category\" ><a href='{0}'>{1}</a> {3}", jstree.data.attributes.href, (jstree.data.attributes.rel != "main") ? jstree.data.title : jstree.data.title.ToUpper(), jstree.data.attributes.rel != "main", (!String.IsNullOrEmpty(jstree.data.icon)) ? "(" + jstree.data.icon + ")" : String.Empty, (jstree.data.attributes.rel != "main") ? " font-weight:bold;color:black" : "font-weight:bold;color:black");

                else
                {
                    objCategoryChildLink.parentLinkUrl = jstree.data.attributes.rel;
                    objCategoryChildLink.parentLinkName = jstree.data.title.ToUpper();
                    objA.parentLinkUrl = jstree.data.attributes.rel;
                    objA.parentLinkName = jstree.data.title.ToUpper();
                    sb.AppendFormat("<li ischild='{1}' ><span style='{3}'>{0}</span> {2}", (jstree.data.attributes.rel != "main") ? jstree.data.title : jstree.data.title.ToUpper(), jstree.data.attributes.rel != "main", (!String.IsNullOrEmpty(jstree.data.icon)) ? "(" + jstree.data.icon + ")" : String.Empty, (jstree.data.attributes.rel != "main") ? " font-weight:normal;color:#0072B0;" : "font-weight:bold;color:black");
                }
                //sb.AppendFormat("<li ischild='{1}' ><span style='{3}'>{0}</span> {2}", (jstree.data.attributes.rel != "main") ? jstree.data.title : jstree.data.title.ToUpper(), jstree.data.attributes.rel != "main", (!String.IsNullOrEmpty(jstree.data.icon)) ? "(" + jstree.data.icon + ")" : String.Empty, (jstree.data.attributes.rel != "main") ? " font-weight:bold;color:black;" : "font-weight:bold;color:black");

                if (jstree.children != null && jstree.children.Count() > 0)
                {
                    objCategoryChildLink.Parount = jstree.children.Count();
                    //sb.Append("<ul>");
                    foreach (JsTreeNode js in jstree.children)
                    {
                        objA1 = new CategoryChild();
                        GetCategoriesMapTreeChild(sb, js, evnt, demo, objCategoryChildLink, objA1);
                        objA.Childs.Add(objA1);
                        //objA.
                    }
                    objCategoryChildLinkAll.Add(objCategoryChildLink);
                    //sb.Append("</ul>");
                }
                sb.AppendFormat("</li>");
                objCategoryChildLinkParent.Add(objCategoryChildLink);
                lstCategoryParentChild.Add(objA);
                //if (jstree.data.attributes.rel == "main") sb.Append("<li><hr /></li>");

            }
            sb.Append("</ul>");

           // IComparer<CategoryParentChild> comparer = new IComparer<CategoryParentChild>();
            List<CategoryParentChild> objListOrder = lstCategoryParentChild.OrderBy(order => order.parentLinkName).ToList();
            HttpContext.Current.Session["lstCategoryParentChild"] = null;
            HttpContext.Current.Session["lstCategoryParentChild"] = objListOrder;
            result = sb.ToString();
            if (!String.IsNullOrEmpty(sb.ToString()))
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result; //new { html = result};
        }


        private List<CategoryParentChild> GetCategoriesMapTree(long event_id, bool demo, bool isaccessable, string strChk = null)
        {
            
            List<CategoryParentChild> lstCategoryParentChild = new List<CategoryParentChild>();
            List<CategoryChildLink> objCategoryChildLinkParent = new List<CategoryChildLink>();
            CategoryParentChild obj = new CategoryParentChild();
            List<CategoryChild> obj1 = new List<CategoryChild>();
            DataCacheObject dco = (!demo) ? new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORIESMAPTREE", new object[] { event_id, isaccessable }, CachingExpirationTime.Days_01) : new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORIESMAPTREEPREVIEW", new object[] { event_id }, CachingExpirationTime.Days_01);
            object result = CacheRepository.Get(dco);
          //  if (result != null) return lstCategoryParentChild; //new { html = result };
            List<JsTreeNode> nodesList = new List<JsTreeNode>();
            JsTreeNode node, nodeParent;

            //event_id = 313;

            var map = (from EC in dataContext.EventCategories
                       where EC.Event_ID == event_id && EC.IsActive && EC.CategoriesMap.Category.IsActive //&& EC.Auctions.Count() > 0
                       orderby ((EC.CategoriesMap.Parent_ID.HasValue) ? EC.CategoriesMap.CategoriesMap_Parent.Category.Title : EC.CategoriesMap.Category.Title) ascending, EC.CategoriesMap.Category.Title ascending
                       select new { CategoryMap = EC.CategoriesMap, AuctionCount = EC.Auctions.Count(), EventCategory = EC }).ToList();
            Dictionary<long, JsTreeNode> tree = new Dictionary<long, JsTreeNode>();
            CategoriesMap tmp;
            bool alreadyInTree;
            foreach (var c in map)
            {
                if (c.AuctionCount == 0 && c.CategoryMap.CategoriesMap_Children.Count() == 0) continue;
                tmp = c.CategoryMap;
                alreadyInTree = tree.ContainsKey(tmp.ID);
                node = (alreadyInTree) ? tree[tmp.ID] : new JsTreeNode();
                node.attributes = new Attributes();
                node.attributes.id = tmp.ID.ToString();
                node.data = new Data();
                node.data.title = tmp.Category.Title;
                node.data.icon = String.Empty;
                node.data.attributes = new Attributes();
                //AppHelper.GetSiteUrl() + 
                node.data.attributes.href = "/" + ((demo) ? "Preview" : "Auction") + "/EventCategory/" + c.EventCategory.ID.ToString() + "/" + UrlParser.TitleToUrl(c.EventCategory.FullCategory);
                node.data.attributes.rel = "main";
                if (alreadyInTree) continue;
                tree.Add(tmp.ID, node);
                while (tmp.Parent_ID.HasValue)
                {
                    tmp = tmp.CategoriesMap_Parent;
                    alreadyInTree = tree.ContainsKey(tmp.ID);
                    nodeParent = (alreadyInTree) ? tree[tmp.ID] : new JsTreeNode();
                    nodeParent.attributes = new Attributes();
                    nodeParent.attributes.id = tmp.ID.ToString();
                    nodeParent.data = new Data();
                    nodeParent.data.title = tmp.Category.Title;
                    nodeParent.data.attributes = new Attributes();
                    //AppHelper.GetSiteUrl() + 
                    nodeParent.data.attributes.href = "/" + ((demo) ? "Preview" : "Auction") + "/Category/" + tmp.ID.ToString() + "/" + UrlParser.TitleToUrl(tmp.Category.Title) + "?e=" + event_id.ToString();
                    node.data.attributes.rel = "child";
                    nodeParent.data.attributes.rel = "main";
                    nodeParent.state = "open";
                    if (nodeParent.children == null) nodeParent.children = new List<JsTreeNode>();
                    nodeParent.children.Add(node);
                    if (alreadyInTree) break;
                    node = nodeParent;
                    tree.Add(tmp.ID, nodeParent);
                }
                if (alreadyInTree) continue;
                nodesList.Add(node);
            }
            //return nodesList;
            StringBuilder sb = new StringBuilder();
            Event evnt = dataContext.Events.SingleOrDefault(E => E.ID == event_id);
            CategoryChild objA1 = null;
            
            sb.Append("<ul class=\"list-unstyled\">");
            foreach (JsTreeNode jstree in nodesList)
            {
                CategoryParentChild objA = new CategoryParentChild();

                CategoryChildLink objCategoryChildLink = new CategoryChildLink();
                List<CategoryChildLink> objCategoryChildLinkAll = new List<CategoryChildLink>();
                List<CategoryChildLink> objCategoryChildLink1 = new List<CategoryChildLink>();



                int uji = objA.Childs.Count;
                if (evnt.IsClickable || demo || evnt.IsAccessable)
                {
                    objCategoryChildLink.parentLinkUrl = jstree.data.attributes.href;
                    objCategoryChildLink.parentLinkName = jstree.data.title.ToUpper();
                    objA.parentLinkUrl = jstree.data.attributes.href;
                    objA.parentLinkName = jstree.data.title.ToUpper();
                    sb.AppendFormat("<li ischild='{2}' class=\"title-category\" ><a href='{0}'>{1}</a> {3}", jstree.data.attributes.href, (jstree.data.attributes.rel != "main") ? jstree.data.title : jstree.data.title.ToUpper(), jstree.data.attributes.rel != "main", (!String.IsNullOrEmpty(jstree.data.icon)) ? "(" + jstree.data.icon + ")" : String.Empty, (jstree.data.attributes.rel != "main") ? " font-weight:normal;color:#0072B0;" : "font-weight:bold;color:black");
                }
                //  sb.AppendFormat("<li ischild='{2}' class=\"title-category\" ><a href='{0}'>{1}</a> {3}", jstree.data.attributes.href, (jstree.data.attributes.rel != "main") ? jstree.data.title : jstree.data.title.ToUpper(), jstree.data.attributes.rel != "main", (!String.IsNullOrEmpty(jstree.data.icon)) ? "(" + jstree.data.icon + ")" : String.Empty, (jstree.data.attributes.rel != "main") ? " font-weight:bold;color:black" : "font-weight:bold;color:black");

                else
                {
                    objCategoryChildLink.parentLinkUrl = jstree.data.attributes.rel;
                    objCategoryChildLink.parentLinkName = jstree.data.title.ToUpper();
                    objA.parentLinkUrl = jstree.data.attributes.rel;
                    objA.parentLinkName = jstree.data.title.ToUpper();
                    sb.AppendFormat("<li ischild='{1}' ><span style='{3}'>{0}</span> {2}", (jstree.data.attributes.rel != "main") ? jstree.data.title : jstree.data.title.ToUpper(), jstree.data.attributes.rel != "main", (!String.IsNullOrEmpty(jstree.data.icon)) ? "(" + jstree.data.icon + ")" : String.Empty, (jstree.data.attributes.rel != "main") ? " font-weight:normal;color:#0072B0;" : "font-weight:bold;color:black");
                }
                //sb.AppendFormat("<li ischild='{1}' ><span style='{3}'>{0}</span> {2}", (jstree.data.attributes.rel != "main") ? jstree.data.title : jstree.data.title.ToUpper(), jstree.data.attributes.rel != "main", (!String.IsNullOrEmpty(jstree.data.icon)) ? "(" + jstree.data.icon + ")" : String.Empty, (jstree.data.attributes.rel != "main") ? " font-weight:bold;color:black;" : "font-weight:bold;color:black");

                if (jstree.children != null && jstree.children.Count() > 0)
                {
                    objCategoryChildLink.Parount = jstree.children.Count();
                    //sb.Append("<ul>");
                    foreach (JsTreeNode js in jstree.children)
                    {
                        objA1 = new CategoryChild();
                        GetCategoriesMapTreeChild(sb, js, evnt, demo, objCategoryChildLink, objA1);
                        objA.Childs.Add(objA1);
                        //objA.
                    }
                    objCategoryChildLinkAll.Add(objCategoryChildLink);
                    //sb.Append("</ul>");
                }
                sb.AppendFormat("</li>");
                objCategoryChildLinkParent.Add(objCategoryChildLink);
                lstCategoryParentChild.Add(objA);
                //if (jstree.data.attributes.rel == "main") sb.Append("<li><hr /></li>");

            }
            sb.Append("</ul>");

            // IComparer<CategoryParentChild> comparer = new IComparer<CategoryParentChild>();
            List<CategoryParentChild> objListOrder = lstCategoryParentChild.OrderBy(order => order.parentLinkName).ToList();
           // HttpContext.Current.Session["lstCategoryParentChild"] = null;
           // HttpContext.Current.Session["lstCategoryParentChild"] = objListOrder;
            result = sb.ToString();
            if (!String.IsNullOrEmpty(sb.ToString()))
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return lstCategoryParentChild; //new { html = result};
        }

        //GetCategoriesMapTreeJSON
        public object GetCategoriesMapTreeJSON(long event_id, bool demo)
        {
            //StringBuilder sb = new StringBuilder();
            IEventRepository er = new EventRepository(dataContext, CacheRepository);
            Event evnt = er.GetEventByID(event_id);
            //string pref = (evnt.IsClickable || demo || evnt.IsAccessable) ? (demo?"d" : "a") : "n";
            //string file = String.Format(@"~\Templates\Different\{0}{1}.htm", pref, event_id);
            //FileInfo fi = new FileInfo(System.Web.HttpContext.Current.Server.MapPath(file));
            //if (fi.Exists)
            //{
            //  TextReader fileReader = new StreamReader(fi.FullName);
            //  sb.Append(fileReader.ReadToEnd());
            //  fileReader.Close();
            //  return (sb.Length > 0) ? new { html = sb.ToString() } : GetCategoriesMapTree(event_id, demo, evnt.IsClickable || demo || evnt.IsAccessable);
            //}
            return GetCategoriesMapTree(event_id, demo, evnt.IsClickable || demo || evnt.IsAccessable);
        }
        #endregion


        //GetCategoriesMapTreeJSON
        public List<CategoryParentChild> GetCategoriesMapTreeJSON(long event_id, bool demo, string strChk = null)
        {
            //StringBuilder sb = new StringBuilder();
            IEventRepository er = new EventRepository(dataContext, CacheRepository);
            Event evnt = er.GetEventByID(event_id);
            //string pref = (evnt.IsClickable || demo || evnt.IsAccessable) ? (demo?"d" : "a") : "n";
            //string file = String.Format(@"~\Templates\Different\{0}{1}.htm", pref, event_id);
            //FileInfo fi = new FileInfo(System.Web.HttpContext.Current.Server.MapPath(file));
            //if (fi.Exists)
            //{
            //  TextReader fileReader = new StreamReader(fi.FullName);
            //  sb.Append(fileReader.ReadToEnd());
            //  fileReader.Close();
            //  return (sb.Length > 0) ? new { html = sb.ToString() } : GetCategoriesMapTree(event_id, demo, evnt.IsClickable || demo || evnt.IsAccessable);
            //}
            return GetCategoriesMapTree(event_id, demo, evnt.IsClickable || demo || evnt.IsAccessable, strChk);
        }


        //GetEventCategoryById
        public EventCategory GetEventCategoryById(long id)
        {
            DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETEVENTCATEGORYBYID", new object[] { id }, CachingExpirationTime.Days_01);
            EventCategory result = CacheRepository.Get(dco) as EventCategory;
            if (result != null) return result;
            result = dataContext.spSelect_EventCategory(id).FirstOrDefault();
            if (result != null)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetFullEventCategoryLink
        public string GetFullEventCategoryLink(long eventcategory_id)
        {
            DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "FULLCATEGORYLINK_EC", new object[] { eventcategory_id }, CachingExpirationTime.Days_01);
            string result = CacheRepository.Get(dco) as string;
            if (!String.IsNullOrEmpty(result)) return result;
            EventCategory ec = dataContext.spSelect_EventCategory(eventcategory_id).FirstOrDefault();
            if (ec == null) return "---";
            result = ec.FullCategoryLink;
            if (!string.IsNullOrEmpty(result))
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetCategoryMapById
        public CategoriesMap GetCategoryMapById(long id)
        {
            DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORYMAPBYID", new object[] { id }, CachingExpirationTime.Days_01);
            CategoriesMap result = CacheRepository.Get(dco) as CategoriesMap;
            if (result != null) return result;
            result = dataContext.spSelect_CategoriesMap(id).FirstOrDefault();
            if (result != null)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetCategoryLeafs
        public List<IdTitle> GetCategoryLeafs(long? category_id)
        {
            DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORYLEAFS", new object[] { category_id }, CachingExpirationTime.Days_01);
            List<IdTitle> result = CacheRepository.Get(dco) as List<IdTitle>;
            if (result != null && result.Any()) return result;
            dataContext.CommandTimeout = 600000;
            result = (from p in dataContext.spCategory_View_FullCategory(category_id)
                      select new IdTitle
                      {
                          ID = p.Category_ID.GetValueOrDefault(0),
                          Title = p.FullCategory
                      }).ToList();
            if (result.Any())
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetAllowedCategoriesForTheEvent
        public List<EventCategoryDetail> GetAllowedCategoriesForTheEvent(long event_id)
        {
            DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETALLOWEDCATEGORIESFORTHEEVENT", new object[] { event_id }, CachingExpirationTime.Days_01);
            List<EventCategoryDetail> result = CacheRepository.Get(dco) as List<EventCategoryDetail>;
            if (result != null && result.Any()) return result;
            result = (from p in dataContext.spEventCategory_ListForEvent(event_id)
                      select new EventCategoryDetail
                               {
                                   EventCategory_ID = p.EventCategory_ID,
                                   Category_ID = p.Category_ID,
                                   CategoryDescription = p.CategoryDescription,
                                   CategoryTitle = p.CategoryTitle,
                                   CategoryMap_ID = p.CategoryMap_ID,
                                   IsActive = p.IsActive,
                                   Event_ID = p.Event_ID,
                                   IsTaxable = p.IsTaxable
                               }).ToList();
            if (result.Any())
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetEventCategoryDetail
        public EventCategoryDetail GetEventCategoryDetail(long eventcategory_id)
        {
            DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETEVENTCATEGORYDETAIL", new object[] { eventcategory_id }, CachingExpirationTime.Days_01);
            EventCategoryDetail result = CacheRepository.Get(dco) as EventCategoryDetail;
            if (result != null) return result;
            result = (from p in dataContext.spEventCategory_Detail(eventcategory_id)
                      select new EventCategoryDetail
                      {
                          EventCategory_ID = p.EventCategory_ID,
                          Category_ID = p.Category_ID,
                          CategoryDescription = p.CategoryDescription,
                          CategoryTitle = p.CategoryTitle,
                          CategoryMap_ID = p.CategoryMap_ID,
                          IsActive = p.IsActive,
                          Event_ID = p.Event_ID,
                          IsTaxable = p.IsTaxable
                      }).FirstOrDefault();
            if (result != null)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetCategoryMapById
        public CategoryMapDetail GetCategoryMapDetail(long id)
        {
            DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORYMAPDETAIL", new object[] { id }, CachingExpirationTime.Days_01);
            CategoryMapDetail result = CacheRepository.Get(dco) as CategoryMapDetail;
            if (result != null) return result;
            result = (from p in dataContext.spCategoryMap_Detail(id)
                      select new CategoryMapDetail
                      {
                          Category_ID = p.Category_ID,
                          CategoryDescription = p.CategoryDescription,
                          CategoryMap_ID = p.CategoryMap_ID,
                          CategoryTitle = p.CategoryTitle
                      }).FirstOrDefault();
            if (result != null)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetCategoryLeafs
        public IEnumerable<IdTitle> GetCategories()
        {
            return dataContext.vwCategoriesMap_Fulls.Select(t => new IdTitle { ID = t.CategoryMap_ID.GetValueOrDefault(0), Title = t.FullCategory });
        }
    }
}
