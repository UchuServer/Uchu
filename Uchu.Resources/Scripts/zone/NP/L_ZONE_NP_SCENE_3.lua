--------------------------------------------------------------
-- Includes
--------------------------------------------------------------


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------


--------------------------------------------------------------
-- Helper Functions
--------------------------------------------------------------


--------------------------------------------------------------
-- Game Message Handlers
--------------------------------------------------------------

function Scene3Startup(self)
    --moved to CONCERT_STAGE.lua
    --LoadChoiceBuild(self, 4029, "Concert_Prop_1")
    --LoadChoiceBuild(self, 4030, "Concert_Prop_2")
    --LoadChoiceBuild(self, 4031, "Concert_Prop_3")
    --LoadChoiceBuild(self, 4032, "Concert_Prop_4")
   

end



function onScene3OnNotifyObject(self,msg)

    if msg.name == "Dance_Emote" then
    
        
    
    end



end


function LoadChoiceBuild(self, templateID, pathname)
    --print ("inside Load Choice Build Function------------------")
    local pathMsg = LEVEL:GetPathWaypoints (pathname)
	
	if (tostring(type(pathMsg)) == "table") then
		for i, v in pairs(pathMsg) do
            --print ("loading object")
            RESMGR:LoadObject {
                objectTemplate = templateID,
                	x = v.pos.x,
                    y = v.pos.y,
                    z = v.pos.z,
                    rw = v.rot.w,
                    rx = v.rot.x,
                    ry = v.rot.y,
                    rz = v.rot.z,
                    owner = self }
		end
    end
end