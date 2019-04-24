

function onEmoteReceived(self,msg)
    if (msg.emoteID == 356) then 
        self:PlayAnimation{ animationID = "salutePlayer" }
    end
end
