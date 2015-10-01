using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ElmatSvc.Business;
using ElmatSvc.Messages;

namespace ElmatSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RideService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RideService.svc or RideService.svc.cs at the Solution Explorer and start debugging.
    public class RideService : IRideService 
    {
        public string CadastraCarona(Ride R) 
        {
            try
            {
                R = RideBLL.CadastraRide(R);
            }
            catch (Exception)
            {
                return "Ocorreu um erro ao cadastrar a carona";
            }
            if (R.RideID.HasValue)
                return "Carona cadastrada com sucesso";
            else
                return "A carona não foi cadastrada";
        }

        public List<Ride> ListaSolCaronas(FiltroRide busca, User usr, double LatOrg, double LonOrg, double? LatDes, double? LonDes)
        {
            // Lista as caronas disponíveis para o usuário
            List<Ride> Lista = RideBLL.ListaCaronas(busca, usr);
            
            if (!LatDes.HasValue || !LonDes.HasValue)
            {
                Lista = RideBLL.ClassificaCaronasSemRota(Lista, LatOrg, LonOrg);
            }
            else
            {
                //O usuário definiu uma rota com destino, ao avaliar a carona levar em consideração sua localização
                Lista = RideBLL.ClassificaCaronasComRota(Lista, LatOrg, LonOrg, LatDes.Value, LonDes.Value);
            }
            return Lista;
            
        }

        //string AtendeSolicitacaoCarona(User usr, int RideID)
        //{
        //    return "teste"; 
        //}
    }
}
