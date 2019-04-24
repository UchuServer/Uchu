function onFireEventServerSide(self, msg)
    if(msg.args == "AbortWBLZone") then
        msg.senderID:TransferToZone{zoneID = 1600}
    end
end
