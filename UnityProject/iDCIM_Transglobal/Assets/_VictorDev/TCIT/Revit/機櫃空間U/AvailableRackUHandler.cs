using System.Collections.Generic;
using System.Linq;
using _VictorDEV.Revit;
using UnityEngine;
using Debug = VictorDev.Common.Debug;

public class AvailableRackUHandler : MonoBehaviour, DeviceDataManager.IReceiverRackModelDataExtended
{

    public void ReceiverData(List<RackModelDataExtended> data)
    {
        rackModeldatas = data;
        rackModeldatas.ForEach(CreateRackSpaceDisplayer);
    }

    private void CreateRackSpaceDisplayer(RackModelDataExtended rackData)
    {
        rackData.AvailableULevels.ForEach(level =>
        {
            RackSpaceDisplayer displayer = Instantiate(itemPrefab, rackData.Model);
            displayer.ReceiveRackData(rackData);
            displayer.SetULocation(level);
            //displayer.gameObject.SetActive(false);
            rackData.AvailableUDisplayer.Add(displayer);
        });
    }

    private List<RackModelDataExtended> rackModeldatas;
    public RackSpaceDisplayer itemPrefab;
}
