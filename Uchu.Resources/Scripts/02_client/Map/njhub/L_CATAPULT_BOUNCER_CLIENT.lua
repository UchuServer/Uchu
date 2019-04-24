----------------------------------------
-- Server side script on the bouncer pad attached to the catapult
--
-- created by brandi... 6/23/11
----------------------------------------

function onScriptNetworkVarUpdate(self,msg)
	-- parse through the table of network vars that were updated
	for k,v in pairs(msg.tableOfVars) do
		-- start a timer to make bouncer invisible
		if k == "Built" then
			GAMEOBJ:GetTimer():AddTimerWithCancel(1, "MakeInvisible", self)
		end
	end
end


-- make the bouncer invisible
function onTimerDone(self,msg)
	if msg.name == "MakeInvisible" then
		self:SetVisible{visible = false, fadeTime = 0.0}
	end
end

-- the bouncer is about to die, make it visible so the bricks will show up when it smashes
function onNotifyClientObject(self,msg)
	if msg.name == "TimeToDie" then
		self:SetVisible{visible = true, fadeTime = 0.0}
	end
end