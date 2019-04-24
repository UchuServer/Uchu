function onFireEventServerSide(self, msg)
    --print('onFireEventServerSide ' .. msg.args .. ' ' .. msg.senderID:GetName().name .. ' ' .. msg.param1)
    if msg.args == "TransferToInstance" then
        msg.senderID:TransferToZone{ zoneID = msg.param1, ucInstanceType = 1 } --instance type single		
    end
end 