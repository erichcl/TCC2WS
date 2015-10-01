using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ElmatSvc.Messages;

namespace ElmatSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRideService" in both code and config file together.
    [ServiceContract]
    public interface IRideService
    {
        [OperationContract]
        string CadastraCarona(Ride R);
        [OperationContract]
        List<Ride> ListaSolCaronas(FiltroRide busca, User usr, double LatOrg, double LonOrg, double? LatDes, double? LonDes);
        //[OperationContract]
        //string AtendeSolicitacaoCarona(User usr, int RideID);
    }
}
