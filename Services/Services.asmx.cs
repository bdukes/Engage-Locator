//Copyright (c) 2004-2007
//by Engage Software ( http://www.engagesoftware.com )

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//DEALINGS IN THE SOFTWARE.

using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using Engage.Dnn.Locator.Providers.MapProviders;
using Engage.Dnn.Locator.Maps;

namespace Engage.Dnn.Locator.Services
{
    /// <summary>
    /// Summary description for Services
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Services : System.Web.Services.WebService
    {

        [WebMethod]
        public DataTable GetLocationsByZip(string mapProviderTypeName, string apiKey, int radius, string postalCode, int locationTypeId, int portalId)
        {
            int[] locationTypeIds = { locationTypeId };

            return SearchLocationsByZip(mapProviderTypeName, apiKey, radius, postalCode, locationTypeIds, portalId);
        }

        public DataTable GetLocationsByZip(string mapProviderTypeName, string apiKey, int radius, string postalCode, int[] locationTypeIds, int portalId)
        {
            return SearchLocationsByZip(mapProviderTypeName, apiKey, radius, postalCode, locationTypeIds, portalId);
        }

        private DataTable SearchLocationsByZip(string mapProviderTypeName, string apiKey, int radius, string postalCode, int[] locationTypeIds, int portalId)
        {

            //use reflection to construct the provider class.
            MapProviderType providerType = (MapProviderType)MapProviderType.GetFromName(mapProviderTypeName, typeof(MapProviderType));
            //MapProvider provider = MapProvider.CreateInstance(providerType);
            
            DataTable matches = null;

            //figure out which method to call based on provider.
            if (providerType == MapProviderType.GoogleMaps)
            {
                GoogleGeocodeResult result = SearchUtility.SearchGoogle(postalCode, apiKey);
                matches = Data.DataProvider.Instance().GetClosestLocationsByRadius(result.latitude, result.longitude, radius, portalId, locationTypeIds);
            }
            else
            {
                YahooGeocodeResult result = SearchUtility.SearchYahoo("", "", "", "", postalCode, apiKey);
                matches = Data.DataProvider.Instance().GetClosestLocationsByRadius(result.latitude, result.longitude, radius, portalId, locationTypeIds);
            }
            
            return matches;
        }

        [WebMethod]
        public DataTable GetLocation(int locationId)
        {
            return Data.DataProvider.Instance().GetLocation(locationId);
        }
    }
}
