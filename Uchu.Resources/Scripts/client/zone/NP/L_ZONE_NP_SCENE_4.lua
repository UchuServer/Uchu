--------------------------------------------------------------
-- Nimbus Park - Scene 4 Specific Client Zone Script Functions
--------------------------------------------------------------
local courseCollectables = {}
local numCollectables = 0
local numCollected = 0
local numBigCollected = 0


--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function Scene4Startup(self)

    self:SetVar("IsPlayerInCourse", false)  
    
end


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function Scene4OnChildLoaded(self, msg)
	if (msg.templateID == CONSTANTS["CollectableTemplate"]) or (msg.templateID == CONSTANTS["BigCollectableTemplate"]) then
		numCollectables = numCollectables + 1
		courseCollectables[numCollectables] = msg.childID
	end
end


--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function Scene4OnObjectLoaded(self, msg)

	-- check for scene 4 course LOTs
	if (msg.templateID == CONSTANTS["COURSE_STARTER_LOT"]) then
        storeObjectByName(self, "CourseStarter", msg.objectID)
	end	
	
end	


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function Scene4OnTimerDone(self, msg)

    if (msg.name == "CourseGo") then

        -- unpause player
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = false}

        -- record start time
        local startTime = GAMEOBJ:GetSystemTime()
        self:SetVar("CourseStartTime", startTime)

    end   

end


--------------------------------------------------------------
-- Shuts down the course
--------------------------------------------------------------
function Scene4CourseShutdown(self, msg)
	for i = 1, #courseCollectables do
		if courseCollectables[i]:Exists() then
			GAMEOBJ:DeleteObject(courseCollectables[i])
		end
	end
	numCollectables = 0
	numCollected = 0
	numBigCollected = 0
end


--------------------------------------------------------------
-- Starts the course
--------------------------------------------------------------
function Scene4CourseStartup(self)
	local pathMsg = LEVEL:GetPathWaypoints(CONSTANTS["CollectablePath"])
	
	if (tostring(type(pathMsg)) == "table") then
		for i, v in pairs(pathMsg) do
			RESMGR:LoadObject { objectTemplate = CONSTANTS["CollectableTemplate"],
								bIsSmashable = false,
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
	
	pathMsg = LEVEL:GetPathWaypoints(CONSTANTS["BigCollectablePath"])
	
	if (tostring(type(pathMsg)) == "table") then
		for i, v in pairs(pathMsg) do
			RESMGR:LoadObject { objectTemplate = CONSTANTS["BigCollectableTemplate"],
								bIsSmashable = false,
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


--------------------------------------------------------------
-- Generic notification message
--------------------------------------------------------------
function Scene4OnNotifyObject(self, msg)

    -- Scene 4 Course Triggers, player must be in course
    if (self:GetVar("IsPlayerInCourse") == true) then
    
        if (msg.name == "scene_4_course_cancel") then
        
            -- cancel the course
            self:SetVar("IsPlayerInCourse", false)
            --clean up the course
            Scene4CourseShutdown(self, msg)
            
            local starter = getObjectByName(self,"CourseStarter")
            if (starter) then
                starter:SetVar("IsPlayerInCourse", false)
            end
            
            UI:DisplayToolTip
            {
                strDialogText = CONSTANTS["COURSE_OUT_OF_RANGE_TEXT"], 
                strImageName = "", 
                bShow = true, 
                iTime = CONSTANTS["COURSE_OUT_OF_RANGE_MSG_SHOW_TIME"]
            }            
            
        elseif (msg.name == "scene_4_course_finish") then
        
            -- record the time
            local startTime = self:GetVar("CourseStartTime")
            local endTime = GAMEOBJ:GetSystemTime()
            local totalTime = tonumber(endTime) - tonumber(startTime)
            local modifiedTotal = totalTime - numCollected * tonumber(CONSTANTS["CollectableTimeAdded"]) - numBigCollected * tonumber(CONSTANTS["BigCollectableTimeAdded"]) --numCollected * 2 seconds
            if modifiedTotal <= 10 then
				modifiedTotal = 10
			end
            
            -- finish the course
            self:SetVar("IsPlayerInCourse", false)
            
            --clean up the course
            Scene4CourseShutdown(self, msg)
            -- send details to the starter
            local starter = getObjectByName(self,"CourseStarter")
            if (starter) then

                starter:SetVar("IsPlayerInCourse", false)
                
                -- check for best time change
                local curBestTime = tonumber(starter:GetVar("PlayerBestTime"))
                if ( (totalTime < curBestTime) or (curBestTime == 0) ) then
                    starter:SetVar("PlayerBestTime",modifiedTotal)
                end

            end
            
            -- inform the player
            UI:DisplayToolTip
            {
                strDialogText = CONSTANTS["COURSE_FINISH_TEXT"] .. " " .. ParseTime(totalTime) .. " " .. CONSTANTS["COURSE_FINISH_TEXT2"] .. " " .. ParseTime(modifiedTotal), 
                strImageName = "", 
                bShow = true, 
                iTime = CONSTANTS["COURSE_FINISH_MSG_SHOW_TIME"]
            }
            
		elseif msg.name == "CollectableCollected" then
			numCollected = numCollected + 1
		elseif msg.name == "BigCollectableCollected" then
			numBigCollected = numBigCollected + 1
		end
    
    end

    -- start a new course, must not be in one already
    if (msg.name == "scene_4_course_start" and self:GetVar("IsPlayerInCourse") == false) then

        -- pause player
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = true}

        -- start the course
        self:SetVar("IsPlayerInCourse", true)
        
        local starter = getObjectByName(self,"CourseStarter")
        if (starter) then
            starter:SetVar("IsPlayerInCourse", true)
        end
        
        -- trigger the Countdown
        player:ShowActivityCountdown
        {
            bPlayCountdownSound = false,
            bPlayAdditionalSound = false,
        }

        -- set a timer to Unpause and Go
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["COUNTDOWN_TIME"], "CourseGo", self )
        
		Scene4CourseStartup(self)

    end  
end