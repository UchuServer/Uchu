function onFireEventServerSide(self, msg)
    if (msg.args == "ZonePlayer") then
        msg.senderID:TransferToZone{ zoneID = msg.param1 }
        print('zone player')
    end
end