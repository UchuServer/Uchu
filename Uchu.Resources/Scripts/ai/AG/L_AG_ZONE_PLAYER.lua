function onFireEventServerSide(self, msg)
    if (msg.args == "ZonePlayer") then
        msg.senderID:TransferToZone{ zoneID = msg.param1 }
    end
end