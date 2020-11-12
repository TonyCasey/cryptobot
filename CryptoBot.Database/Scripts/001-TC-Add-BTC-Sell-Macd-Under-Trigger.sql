
BEGIN TRAN


IF EXISTS(
		SELECT * 
		From Bot.RuleSets rs
		INNER JOIN	Bot.Rules r 
		ON r.RuleSet_RuleSetId = rs.RuleSetId
		WHERE 
		r.IndicatorRule = 12 AND rs.Indicator_IndicatorId = 1 )

    BEGIN
        PRINT 'Already ran, skipping it'
		COMMIT TRAN
    END

ELSE    
    
	BEGIN TRY 
        PRINT N'Running script:'

		-- Custom SQL to run below here
			
			DECLARE @newRulesetId int

			INSERT INTO 
			Bot.RuleSets 
			(RuleSide, CreationTime, CreationUser, LastUpdateTime, LastUpdateUser, Indicator_IndicatorId, Safety_SafetyId ) 
			VALUES 
			(2,	GETDATE(),	'Script', NULL,	NULL, 1, NULL)

			SET @newRulesetId = SCOPE_IDENTITY()

			INSERT INTO 
			Bot.Rules
			(IndicatorRule, Value, CreationTime, CreationUser, LastUpdateTime, LastUpdateUser, RuleSet_RuleSetId)
			VALUES
			(12, NULL,	GETDATE(), 'Script', NULL, NULL, @newRulesetId)
			
 
        -- End of custom SQL here
		
		PRINT 'Committing...'
        COMMIT TRAN
						
		PRINT 'Script ran successfully...'

    END TRY

    BEGIN CATCH

        PRINT 'Error running script: '
        PRINT 'An error occurred: ' + ERROR_MESSAGE()
        PRINT 'Rolling back transaction for script '

        IF(@@TRANCOUNT > 0)
            ROLLBACK TRAN

    END CATCH

GO

BEGIN TRAN

Declare @botId int, @indicatorId int

Set @botId = 2 -- ETH
Set @indicatorId = 2 -- ETH MACD

IF EXISTS(
		SELECT * 
		From Bot.RuleSets rs
		INNER JOIN	Bot.Rules r 
		ON r.RuleSet_RuleSetId = rs.RuleSetId
		WHERE 
		r.IndicatorRule = 12 AND rs.Indicator_IndicatorId = @indicatorId )

    BEGIN
        PRINT 'Already ran, skipping it'
		COMMIT TRAN
    END

ELSE    
    
	BEGIN TRY 
        PRINT N'Running script:'

		-- Custom SQL to run below here
			
			DECLARE @newRulesetId int

			INSERT INTO 
			Bot.RuleSets 
			(RuleSide, CreationTime, CreationUser, LastUpdateTime, LastUpdateUser, Indicator_IndicatorId, Safety_SafetyId ) 
			VALUES 
			(2,	GETDATE(),	'Script', NULL,	NULL, @indicatorId, NULL)

			SET @newRulesetId = SCOPE_IDENTITY()

			INSERT INTO 
			Bot.Rules
			(IndicatorRule, Value, CreationTime, CreationUser, LastUpdateTime, LastUpdateUser, RuleSet_RuleSetId)
			VALUES
			(12, NULL,	GETDATE(), 'Script', NULL, NULL, @newRulesetId)
			
 
        -- End of custom SQL here
		
		PRINT 'Committing...'
        COMMIT TRAN
						
		PRINT 'Script ran successfully...'

    END TRY

    BEGIN CATCH

        PRINT 'Error running script: '
        PRINT 'An error occurred: ' + ERROR_MESSAGE()
        PRINT 'Rolling back transaction for script '

        IF(@@TRANCOUNT > 0)
            ROLLBACK TRAN

    END CATCH

GO




BEGIN TRAN

Declare @botId int, @indicatorId int

Set @botId = 3 -- LTC
Set @indicatorId = 3 -- LTC MACD

IF EXISTS(
		SELECT * 
		From Bot.RuleSets rs
		INNER JOIN	Bot.Rules r 
		ON r.RuleSet_RuleSetId = rs.RuleSetId
		WHERE 
		r.IndicatorRule = 12 AND rs.Indicator_IndicatorId = @indicatorId )

    BEGIN
        PRINT 'Already ran, skipping it'
		COMMIT TRAN
    END

ELSE    
    
	BEGIN TRY 
        PRINT N'Running script:'

		-- Custom SQL to run below here
			
			DECLARE @newRulesetId int

			INSERT INTO 
			Bot.RuleSets 
			(RuleSide, CreationTime, CreationUser, LastUpdateTime, LastUpdateUser, Indicator_IndicatorId, Safety_SafetyId ) 
			VALUES 
			(2,	GETDATE(),	'Script', NULL,	NULL, @indicatorId, NULL)

			SET @newRulesetId = SCOPE_IDENTITY()

			INSERT INTO 
			Bot.Rules
			(IndicatorRule, Value, CreationTime, CreationUser, LastUpdateTime, LastUpdateUser, RuleSet_RuleSetId)
			VALUES
			(12, NULL,	GETDATE(), 'Script', NULL, NULL, @newRulesetId)
			
 
        -- End of custom SQL here
		
		PRINT 'Committing...'
        COMMIT TRAN
						
		PRINT 'Script ran successfully...'

    END TRY

    BEGIN CATCH

        PRINT 'Error running script: '
        PRINT 'An error occurred: ' + ERROR_MESSAGE()
        PRINT 'Rolling back transaction for script '

        IF(@@TRANCOUNT > 0)
            ROLLBACK TRAN

    END CATCH

GO

