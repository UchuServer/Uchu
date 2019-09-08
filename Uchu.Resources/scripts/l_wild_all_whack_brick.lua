--function onStartup(self)
--end

require('o_mis')

function onStartup(self)
    local Points = {}
    local MainPos = self:GetPosition().pos

    Points[1] = { x = MainPos.x , y = MainPos.y, z = MainPos.z }
    Points[2] = { x = MainPos.x + 10 , y = MainPos.y, z = MainPos.z}
    Points[3] = { x = MainPos.x + 10 , y = MainPos.y, z = MainPos.z + 10}
    Points[4] = { x = MainPos.x + 10 , y = MainPos.y, z = MainPos.z - 10}
    Points[5] = { x = MainPos.x - 10 , y = MainPos.y, z = MainPos.z}
    Points[6] = { x = MainPos.x - 10 , y = MainPos.y, z = MainPos.z + 10}
    Points[7] = { x = MainPos.x, y = MainPos.y, z = MainPos.z + 10}
    Points[8] = { x = MainPos.x, y = MainPos.y, z = MainPos.z - 10}

    for i = 1, 8 do 
        self:SetVar("Points_"..i, Points[i] )
    end

    local ran = math.random(1,8) 
    local ranTime = math.random(1,6) 
    self:Teleport{ pos = self:GetVar("Points_"..ran ) }
    GAMEOBJ:GetTimer():AddTimerWithCancel( ranTime , "bricktimer", self )

end

onTimerDone = function(self, msg) 

    if msg.name == "bricktimer" then
       
            local ran = math.random(1,8) 
            local ranTime = math.random(1,6)
            local pos_tele = {}
            local pos_tele = self:GetVar("Points_"..ran )
            self:Teleport{ pos =  pos_tele }
            GAMEOBJ:GetTimer():AddTimerWithCancel( ranTime, "emote", self )
 
    end 
    
    if msg.name == "emote" then
     Emote.emote(self, self, "down" )
     GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "bricktimer", self )
    end 
    
    
    

end 

