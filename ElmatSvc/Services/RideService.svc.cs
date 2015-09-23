using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ElmatSvc.Business;
using ElmatSvc.Messages;

namespace ElmatSvc.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RideService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RideService.svc or RideService.svc.cs at the Solution Explorer and start debugging.
    public class RideService : IRideService
    {
        string CadastraCarona(Ride R) 
        {
            try
            {
                R = RideBLL.CadastraRide(R);
            }
            catch (Exception e)
            {
                return "Ocorreu um erro ao cadastrar a carona";
            }
            if (R.RideID.HasValue)
                return "Carona cadastrada com sucesso";
            else
                return "A carona não foi cadastrada";
        }

        List<Ride> ListaSolicitacoesCarona(FiltroRide busca, User friend)
        {
            return RideBLL.ListaCaronas(busca, friend);
        }
    }
}
