
function onStartup(self)

	GAMEOBJ:GetZoneControlID():NotifyObject{ name="Zone_Mark", ObjIDSender = self }
	self:SetVar("TimesGateOpened", 0 ) 
end



--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)


	if msg.name == "Start_Join" then
	
		self:SetVar("JTime", self:GetVar("JTime") - 1)
		
		if self:GetVar("JTime") >= 1 then
		
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "Start_Join", self )
			self:NotifyClientObject{name = "JoinTime" , param1 = self:GetVar("JTime") }
		
		else
		
			self:NotifyClientObject{name = "JoinTime" , param1 = self:GetVar("JTime") }
		
		end
		
	
	end

	if msg.name == "Stop_Join" then
	
	
	
	end

	if msg.name == "GameRoundTime" then
	
		self:SetVar("TotalRoundTime", self:GetVar("TotalRoundTime") - 1)
		
		if self:GetVar("TotalRoundTime") >= 0 then
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "GameRoundTime", self )
			self:NotifyClientObject{name = "RoundTime" , param1 = self:GetVar("TotalRoundTime") , paramStr = "round" }
			GAMEOBJ:GetZoneControlID():SetVar("CurrentTime",self:GetVar("TotalRoundTime") )

		else
				if self:GetVar("TimesGateOpened") == 2 then
			
					
					
					GAMEOBJ:GetZoneControlID():NotifyObject{ name="timeExpired", ObjIDSender = self }
				else
				
					local RoundTime = GAMEOBJ:GetZoneControlID():GetVar("Set.Pre_Game_Start")
					self:SetVar("TotalRoundTime" , RoundTime)
					self:NotifyClientObject{name = "RoundTime" , param1 = 0 }
					GAMEOBJ:GetZoneControlID():SetVar("CurrentTime",self:GetVar("TotalRoundTime") )
					--GAMEOBJ:GetZoneControlID():NotifyObject{ name="EndRound", ObjIDSender = self }
					 
					GAMEOBJ:GetZoneControlID():NotifyObject{ name="timeExpired", ObjIDSender = self }
					
				end
		
		end
		
	end

	if msg.name == "GateRoundTime" then
	
		self:SetVar("TotalGateTime", self:GetVar("TotalGateTime") - 1)
		
		if self:GetVar("TotalGateTime") >= 0 then
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "GateRoundTime", self )
			self:NotifyClientObject{name = "RoundTime" , param1 = self:GetVar("TotalGateTime") , paramStr = "Gate"}
			
			
		else
		
				-- Gate time is expired --  Open Game  -- 
				self:NotifyClientObject{ name = "ShowHUD"  }
				self:NotifyClientObject{name = "RoundTime" , param1 = 0 }
				GAMEOBJ:GetZoneControlID():NotifyObject{ name="EndGateTime", ObjIDSender = self }
				for i = 1, #self:GetObjectsInGroup{ group = "gate"}.objects do
					local gate = self:GetObjectsInGroup{ group = "gate"}.objects[i]
				
					gate:GoToWaypoint{iPathIndex = 1}
				end
				
			
		
		end
		
	end

end

function onNotifyObject(self, msg)
 
 	if msg.name == "StopTimer" then
 	
 		GAMEOBJ:GetTimer():CancelAllTimers( self )
 		self:SetVar("TimesGateOpened", 0 ) 
 		
 		
 	end
 
	if msg.name == "StartTimer" then
	
		local Defend = GAMEOBJ:GetZoneControlID():GetVar("Set.DefendTime")
		self:SetVar("TotalRoundTime" , Defend)
	
		self:SetVar("Round_Time", parama1 )
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "GameRoundTime", self )
	
	end
	 -- (3) -- 
	if msg.name == "StartGateTimer" then
		local GateTimer = GAMEOBJ:GetZoneControlID():GetVar("Set.Prestart_Time")
		self:SetVar("TotalGateTime" , GateTimer)
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "GateRoundTime", self )
		
		
		self:NotifyClientObject{name = "RoundTime" , param1 = GateTimer }
		self:SetVar("TimesGateOpened", self:GetVar("TimesGateOpened") + 1 ) 
	end
	
	
	if msg.name == "StartJoinTimer" then
	
		--local time = GAMEOBJ:GetZoneControlID():GetVar("Set.Join_Timer")
		--self:SetVar("JTime" , time)
		
		--self:NotifyClientObject{name = "JoinTime" , param1 = time }
		--GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "Start_Join", self )
	
	end

end