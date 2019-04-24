function onStartup(self)

    self:SetProximityRadius{radius = 20}

end

function onProximityUpdate(self, msg)

    if ( msg.status == "ENTER" ) then
      
      if ( GAMEOBJ:GetLocalCharID() == msg.objId:GetID() ) then
      
        msg.objId:DisplayTooltip{ bShow = true, strText = "This is a bouncer, you can bounce on it.", iTime = 5000 }

      end
    
    end

end
