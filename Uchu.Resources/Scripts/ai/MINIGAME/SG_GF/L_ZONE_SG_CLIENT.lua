--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
                
--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self)
	self:SetVar("TotalScore", 0 )
	self:SetVar("rewardModels", {} )
	
    UI:SendMessage( "MinimizeChat", {} )
	UI:SendMessage( "DisableSpeedChat", {} )
end

function onPlayerReady(self) 
	UI:SendMessage( "pushGameState", {{"state", "shootinggallery" }} )	
    self:SetVar("PlayerReady", true)
	
	checkEverythingReady(self)
end

function checkEverythingReady(self)
    local cannonclient = self:GetVar("Cannon_ClientOBJ")
    local ready = self:GetVar("PlayerReady")
    
    if (cannonclient ~= nil and ready ~= nil and ready == true) then
        self:ActivityStateChangeRequest{wsStringValue='clientready'}
        --Notify the server we're ready to enter the activity
    end
end

function onNotifyClientObject(self,msg)	
    if msg.name == "storeCannonClient" then
        self:SetVar("Cannon_ClientOBJ", msg.paramObj:GetID())
        checkEverythingReady(self)
    end 
end

function onShutdown(self, msg)
	UI:SendMessage( "EnableSpeedChat", {} )
	UI:SendMessage( "popGameState", { {"state", "shootinggallery" } } )	
end