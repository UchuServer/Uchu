--banana cluster spawn from hit tree
require('TestAndExample/MessageNotification/notificationDemoHelpers')


function onStartup(self)
    --can this carry over to another function with a server script, wtih multiple versions of this script running at the same time?
    
    spawnBanana(self)
end


function spawnBanana(self)
	local oPos = { pos = "", rot = ""}
	local oDir = self:GetObjectDirectionVectors()
	oPos.pos = self:GetPosition().pos
    oPos.pos.y = oPos.pos.y + 12
	oPos.pos.x = oPos.pos.x - (oDir.left.x * 5) 
	oPos.pos.z = oPos.pos.z - (oDir.left.z * 5) 
    oPos.rot = self:GetRotation()
	
	local group = "bc"..self:GetVar("number")
	
	--print(group)
	
	local config = { { "groupID" , group } }
    
	RESMGR:LoadObject { objectTemplate = 6909, x= oPos.pos.x, y= oPos.pos.y , z= oPos.pos.z, rw = oPos.rot.w, rx = oPos.rot.x, ry = oPos.rot.y, rz = oPos.rot.z, owner = self, configData = config}

end

function onChildLoaded( self, msg ) 

	local floatingbanana = self:GetObjectsInGroup{group = "bc"..self:GetVar("number"), ignoreSpawners = true}.objects[1]
	storeObjectByName(self,"floatingbanana",floatingbanana)
	--print(#floatingbanana)
	self:SendLuaNotificationRequest{requestTarget=floatingbanana, messageName="Die"}


end
--on hit
function onOnHit(self, msg)
	
	self:SetHealth{health = 9999}
	
	local group = "bc"..self:GetVar("number")
	local clustergroup = self:GetObjectsInGroup{ group = group, ignorespawners = true }.objects  
	
	local bgroup = "FallB"..self:GetVar("number")
	local bconfig = { { "groupID" , bgroup } }
	
	--print(#clustergroup)
	
	--if banana cluster exists
	if #clustergroup == 0 then return end
		--print("clustergroup")

	--delete banana cluster in the tree 
	for k,v in ipairs(clustergroup) do    
        --GAMEOBJ:DeleteObject(v)
		v:Die{ killType = "SILENT" }
    end
	
	local oPos = { pos = "", rot = ""}
	local oDir = self:GetObjectDirectionVectors()
	oPos.pos = self:GetPosition().pos
    oPos.pos.y = oPos.pos.y + 15
	oPos.pos.x = oPos.pos.x - (oDir.left.x * 5)
	oPos.pos.z = oPos.pos.z - (oDir.left.z * 5)
    oPos.rot = self:GetRotation()
	
	--Spawn in falling banana config data 6718 
	local bgroup = "FallB"..self:GetVar("number")
	local bconfig = { { "groupID" , bgroup } }
	RESMGR:LoadObject { objectTemplate = 6718, x= oPos.pos.x, y= oPos.pos.y , z= oPos.pos.z, rw = oPos.rot.w, rx = oPos.rot.x, ry = oPos.rot.y, rz = oPos.rot.z, owner = obj, configData = bconfig}


	--GAMEOBJ:GetTimer():AddTimerWithCancel( 30, "bananaTimer", self ) 
--start timer


end

function notifyDie(self,other,msg)
	--print("i have death")
	GAMEOBJ:GetTimer():AddTimerWithCancel( 30, "bananaTimer", self ) 

end

--on timer done
function onTimerDone(self, msg)
    if msg.name == "bananaTimer" then
	--respawn banana cluster in tree 6909 put it back in that group
	spawnBanana(self)
	
	end
	
end









----on the banana cluster script 
---- send ondie message
