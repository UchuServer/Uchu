----------------------------------------
-- client side script on the the prisoner in cages in the caves
--
-- created by brandi... 6/28/11
----------------------------------------


local Villagers = { [15851] = 1,
					[15866] = 85,
					[15922] = 3,
					[15924] = 4,
					[15927] = 5,
					[15929] = 6}
-- table of chat strings for the villagers to say before they escape the cage

local chatStrings = {
						villager1 = {	"NINJAGO_VILLAGER1_CHAT1",
										"NINJAGO_VILLAGER1_CHAT2",
										"NINJAGO_VILLAGER1_CHAT3",	
										"NINJAGO_VILLAGER1_CHAT4",
										"NINJAGO_VILLAGER1_CHAT5"
									},
						villager2 = {	"NINJAGO_VILLAGER2_CHAT1",
										"NINJAGO_VILLAGER2_CHAT2",
										"NINJAGO_VILLAGER2_CHAT3",	
										"NINJAGO_VILLAGER2_CHAT4",
										"NINJAGO_VILLAGER2_CHAT5" 
									},
						villager3 = {	"NINJAGO_VILLAGER3_CHAT1",
										"NINJAGO_VILLAGER3_CHAT2",
										"NINJAGO_VILLAGER3_CHAT3",	
										"NINJAGO_VILLAGER3_CHAT4",
										"NINJAGO_VILLAGER3_CHAT5"
									},
						villager4 = {	"NINJAGO_VILLAGER4_CHAT1",
										"NINJAGO_VILLAGER4_CHAT2",
										"NINJAGO_VILLAGER4_CHAT3",	
										"NINJAGO_VILLAGER4_CHAT4",
										"NINJAGO_VILLAGER4_CHAT5"
									},
						villager5 = {	"NINJAGO_VILLAGER5_CHAT1",
										"NINJAGO_VILLAGER5_CHAT2",
										"NINJAGO_VILLAGER5_CHAT3",	
										"NINJAGO_VILLAGER5_CHAT4",
										"NINJAGO_VILLAGER5_CHAT5"
									},
						villager6 = {	"NINJAGO_VILLAGER6_CHAT1",
										"NINJAGO_VILLAGER6_CHAT2",
										"NINJAGO_VILLAGER6_CHAT3",	
										"NINJAGO_VILLAGER6_CHAT4",
										"NINJAGO_VILLAGER6_CHAT5"
									}
					}
					

function onNotifyClientObject(self,msg)
	if msg.name == "TimeToChat" then
		-- get name
		local LOT = self:GetLOT().objtemplate
		
		local villNum = Villagers[LOT]
		if not villNum then return end
		local villagerTable = chatStrings["villager"..Villagers[LOT]]
		if not villagerTable then return end
		-- use random to get a chat string from the table
		local ChatString = math.random(table.maxn(villagerTable))
		self:DisplayChatBubble{wsText = Localize(villagerTable[ChatString])} 
	end
end

function onRenderComponentReady(self, msg) 

	local player = GAMEOBJ:GetControlledID()
	if not player:Exists() then return end
	CheckFlag(self,player)
	-- set the random seed
	math.randomseed(os.time())
end

function onScopeChanged(self,msg)
	-- if the player entered ghosting range
    if msg.bEnteredScope then  
	-- get the player
		local player = GAMEOBJ:GetControlledID()
		if not player:Exists() then 
			-- tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- custom function
		CheckFlag(self,player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetControlledID()
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckFlag(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

----------------------------------------------
-- decide to hide the X or not
----------------------------------------------
function CheckFlag(self,player)
	-- if the player is not on the mission, or has used this X, hid it
	local preConVar = self:GetParentObj().objIDParent:GetVar("CheckPrecondition")
	local check = player:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
	
	-- dont let the playe use this if the minigame is active or they dont meet the precondition check.
	if not check.bPass  then
		self:SetVisible{visible = false, fadeTime = 0.0}
	else
		self:SetVisible{visible = true, fadeTime = 0.0}
	end

end