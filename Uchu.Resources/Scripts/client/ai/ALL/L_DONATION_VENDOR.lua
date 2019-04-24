-- ================================================
-- L_DONATION_VENDOR.lua
-- Client Side for the Donation Vendor Plaque
-- updated 12/09/10 Val ... ensuring the plaque gets its initial values regardless of the order in which objects load in
-- ================================================

local myPriority = 0.8
local donationVendorComponent = 100

 function split(str, pat) 
     local t = {} 
      
     if str and pat then 
         string.gsub(str .. pat, "(.-)" .. pat, function(result) table.insert(t, result) end) 
     end 
      
     return t 
 end

function onStartup(self)
	self:SetVar("myActivityID", -1)
	self:SetVar("percentComplete", 0)
	self:SetVar("donationsRemaining", -1)
	
	local groupID = self:GetVar("groupID")
	local groups = split(self:GetVar('groupID'), ';')
	
	-- request info because we don't know in what order things will spawn in
	local members = self:GetObjectsInGroup{ group = groups[1], ignoreSpawners = true, ignoreSelf = true }.objects
	for k,member in pairs(members) do
		if member:HasComponentType{ iComponent = donationVendorComponent }.bHasComponent then
			local donationValues = member:UpdateDonationValues{ }
			self:SetVar("myActivityID", donationValues.activityID)
			self:SetVar("percentComplete", donationValues.fPercentComplete)
			self:SetVar("donationsRemaining", donationValues.totalRemaining)
		end
	end
	
end


-------------------------------------------
-- see if the plaque is in use before allowing the player from using it again
-------------------------------------------
function onCheckUseRequirements(self, msg) 
	if self:GetVar("isInUse")  then	
		msg.bCanUse = false
		
		return msg		
	end
end

----------------------------------------------
-- sent when the object story box is closed;
-- this can be done by hitting the x, esc or enter
----------------------------------------------
function onMessageBoxRespond(self, msg)
   self:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self} 
end


function onGetPriorityPickListType(self, msg)
    if ( myPriority > msg.fCurrentPickTypePriority ) then
       msg.fCurrentPickTypePriority = myPriority
       msg.ePickType = 14    -- Interactive pick type
    end

    return msg
end

function onUpdateDonationValues(self, msg)
	self:SetVar("myActivityID", msg.activityID)
	self:SetVar("percentComplete", msg.fPercentComplete)
	self:SetVar("donationsRemaining", msg.totalRemaining)
	
	--print ('update: ' .. msg.fPercentComplete .. ' ' .. msg.totalRemaining .. ' ' .. self:GetVar("myActivityID"))
	UI:SendMessage("UpdateDonationValues", { {"percentComplete",  msg.fPercentComplete}, {"donationsRemaining", msg.totalRemaining} } )
end

function onClientUse(self, msg)
	if self:GetVar('isInUse') then return end

	UI:SendMessage("pushGameState", { {"state", "CommunityMissionLeaderboard" }, {"context", {{"percentComplete", self:GetVar("percentComplete")}, {"donationsRemaining", self:GetVar("donationsRemaining")}} } })
	UI:SendMessage("ToggleLeaderboard", { {"id", self:GetVar("myActivityID")}, {"visible", true }, {"hideReplay", true}, {"callbackObject", self} } )
	
	self:SetVar('isInUse', true)
end

function onTerminateInteraction(self,msg)
	UI:SendMessage("popGameState", { {"state", "CommunityMissionLeaderboard" } })
	UI:SendMessage("ToggleLeaderboard", { {"id", self:GetVar("myActivityID")}, {"visible", false } } )
	self:SetVar('isInUse', false)
end


