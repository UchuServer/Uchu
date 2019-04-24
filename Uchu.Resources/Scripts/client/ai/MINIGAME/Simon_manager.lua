local color = ''
local round = 1 -- round the player is one
local finalRound = ''
local phase = 0 --phase in the game, this will reset to 0 every round and go up to the number of rounds
local gameStarted = false
local rand = {} --random color table
local randColors = {}
local playerInput = 0 --times the player has collided with a square each turn
local playerTurn = false

function onGetPriorityPickListType(self, msg)
	local myPriority = 0.8
    if ( myPriority > msg.fCurrentPickTypePriority ) then

       msg.fCurrentPickTypePriority = myPriority
       msg.ePickType = 14    -- Interactive pick type 

    end

    return msg

end

function onClientUse(self,msg)
	if gameStarted == false then
		print("you used me")
		DelayBeforeRound(self)
		gameStarted = true
		
		msg.user:SetPlayerControlScheme{iScheme = 3}
		
		
	end
end
function DelayBeforeRound(self)
	--GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):DisplayTooltip { bShow = true, strText = "Round starts in 3.. 2... 1...", iTime = 500 }
	GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):ShowActivityCountdown()
	GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):SetUserCtrlCompPause{bPaused = true}
    GAMEOBJ:GetTimer():AddTimerWithCancel(5, "startRoundIn", self )
end

function PickAColors(self,msg)
	--for i = 1, round do
			
		rand[round] = math.random(1,4)
		
	--end
	for k,j in pairs(rand) do
		print("picking colors "..j.." and i = "..k)
	end
	if #rand > 0 then
	    playColors(self)
	else
	    PickAColors(self,msg)
	end
end

function playColors(self)
	
	if phase < round then
		phase = phase + 1
		local squares = self:GetObjectsInGroup{ group = "Squares", ignoreSpawners = true}.objects
		local oneSquare = squares[rand[phase]]
		--print(GAMEOBJ:GetObjectByID(oneSquare:GetID()))
		print(oneSquare:GetVar('color')) 
		randColors[phase] = oneSquare:GetVar('color')
		oneSquare:NotifyObject{ name = "PlayGameFX" }
		
		
		if phase == round then
		    GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):SetUserCtrlCompPause{bPaused = false}
			playerTurn = true
			for k,v in ipairs(randColors) do
				print(v)
			end
			GAMEOBJ:GetTimer():AddTimerWithCancel(30, "playerTimer", self )
			return
		end	
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "delay", self )
	end
end

function onNotifyClientObject(self, msg) 
	
	if ( msg.name == "playerCollided" ) then
		GAMEOBJ:GetTimer():CancelTimer("playerTimer", self);
		if playerInput < round and gameStarted == true and playerTurn == true then
			playerInput = playerInput + 1
			print ("the player collided with "..msg.paramStr)
			print(randColors[playerInput])
			if randColors[playerInput] == msg.paramStr then
				local group = "Square"..randColors[playerInput]
				print("player was correct and group is "..group)
				local collidedsquare = self:GetObjectsInGroup{ group = "Square"..msg.paramStr ,ignoreSpawners = true}.objects[1]
				collidedsquare:NotifyObject{ name = "PlayerFX" }
				local oPos = self:GetPosition() 
				GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):SetPosition{pos = {x=oPos.pos.x , y=oPos.pos.y, z=oPos.pos.z, }}
				
				
			else
				print("player was wrong")
				GameOver(self)
			end
			if playerInput == round then
				print("player succeded? i think")
				playerTurn = false
				round = round +1
				phase = 0 --phase in the game, this will reset to 0 every round and go up to the number of rounds

				playerInput = 0
				DelayBeforeRound(self)
			end

		
		end
	end
end

function GameOver(self)
	GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):DisplayTooltip { bShow = true, strText = "Game Over try again", iTime = 1000 }
	gameStarted = false
	GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):ResetPlayerControlScheme{}
	round = 1
	phase = 0 --phase in the game, this will reset to 0 every round and go up to the number of rounds

	rand = {} --random color table
	randColors = {}
	playerInput = 0
	print("game stopped")
end
	

function onTimerDone(self,msg)
	if msg.name == "delay" then
		playColors(self)
	end
	if msg.name == "startRoundIn" then
		PickAColors(self)
	end
	if msg.name == "playerTimer" then  
	    GameOver(self)
	end
	if msg.name == "ResetCamera" then
		GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):PlayCinematic{pathName = "SimonCamera" }
		GAMEOBJ:GetTimer():AddTimerWithCancel( 999999 , "ResetCamera" , self )
		GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):SetUserCtrlCompPause{bPaused = false} 
	end
end