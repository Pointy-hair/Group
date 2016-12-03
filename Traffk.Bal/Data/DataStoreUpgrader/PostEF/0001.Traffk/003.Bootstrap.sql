insert into Tenants
(TenantName, LoginDomain, TenantSettings)
values
('Traffk', 't', '{"Deerwalk":{"FtpFolder":"22","FtpHost":"11","FtpPassword":"pp","FtpPort":0,"FtpUser":"33"},"EmailSenderAddress":"jason@jasonthomas.com","EmailSenderName":"Traffk Tenant Admin","ProtectedHealthInformationViewableByEmailAddressHostnames":["a.com","b.com","c.com","d.com","Traffk.com"],"RequiresEmailAccountValidation":false,"RequiresTwoFactorAuthentication":false,"Smtp":{"SmtpHost":"smtp.office365.com","SmtpPassword":"bUSTiT#0JBT","SmtpPort":587,"SmtpUser":"jason@jasonthomas.com"}}')

GO

insert into Applications
(tenantid, applicationtype, applicationname, applicationsettings)
values
(1, 'urn:traffk.com/portal', 'Portal', '{"EmailSenderAddress":"jason@jasonthomas.com","EmailSenderName":"Traffx Portal App Admin","Hosts":{"HostInfos":[{"Hostname":"localhost"},{"Hostname":"traffklocal"}]},"PortalOptions":{"CopyrightMessage":"DB(&copy; 2016 - TraffkPortal)","CssLink":"https:\/\/devhiptraffk.blob.core.windows.net\/portal\/t\/customcss.418493931.css","JavascriptLink":null,"LogoLink":"https:\/\/devhiptraffk.blob.core.windows.net\/portal\/t\/customlogo.png","SupportMessage":"<p>we don''t offer support of any shape or kind!<\/p>","SystemAdminEmailAddress":null,"SystemAdminName":null},"Registration":{"SelfRegistrationMandatoryEmailAddressHostnames":null,"UsersCanSelfRegister":true}}')

GO

insert into templates
(tenantid, TemplateName,	ModelType,	TemplateEngineType,	Code)
values
(1,null, NULL,					'DollarString1',	'Traffk Password Reset'),
(1,null, 'CallbackUrlModel',	'DollarString1',	'Please reset your password by clicking this link: {Model.CallbackUrl}'),
(1,null, 'CallbackUrlModel',	'DollarString1',	'<body style="background-color:pink">Please reset your password by clicking <a href="{Model.CallbackUrl}">here</a>.</body>'),
(1,null, NULL,					'DollarString1',	'Traffk Account Confirmation'),
(1,null, 'CallbackUrlModel',	'DollarString1',	'Please confirm your account by clicking this link: {Model.CallbackUrl}'),
(1,null, 'CallbackUrlModel',	'DollarString1',	'<body style="background-color:yellow">Please confirm your password by clicking <a href="{Model.CallbackUrl}">here</a>.</body>'),
(1,null, NULL,					'DollarString1',	'Traffk Multi-Factor Login Code'),
(1,null, 'SimpleCodeModel',		'DollarString1',	'Your login verification code is {Model.Code}.'),
(1,null, 'SimpleCodeModel',		'DollarString1',	'Your Traffk login verification code is {Model.Code}.'),
(1,null, NULL,					'DollarString1',	'Traffk Invitation Notification'),
(1,null, 'CallbackUrlModel',	'DollarString1',	'Please accept your invitation to join Traffk by clicking this link: {Model.CallbackUrl}

{Model.ContextMessage}'),
(1,null, 'CallbackUrlModel',	'DollarString1',	'<body style="background-color:pink">
Please accept your invitation to join Traffk by clicking <a href="{Model.CallbackUrl}">here</a>.
<div style="background-color:orange">{Model.ContextMessage}</div>
</body>')


GO

insert into messagetemplates
(TenantId, MessageTemplateTitle, SubjectTemplateId, HtmlBodyTemplateId, TextBodyTemplateId)
values
(1, 'Password Reset Email',	1, 3, 2),
(1, 'Account Confirmation Email', 4, 6, 5),
(1, 'Multi-Factor Login Code Email', 7, NULL, 8),
(1, 'Multi-Factor Login Code Sms', NULL, NULL, 9),
(1, 'Account Invitation Email', 10, 12, 11)

GO

insert into systemcommunications
(TenantId, ApplicationId, CommunicationPurpose, CommunicationMedium, MessageTemplateId)
values
(1, NULL, 'System:PasswordReset', 'email', 1),
(1, NULL, 'System:AccountVerification', 'email', 2),
(1, NULL, 'System:TwoFactorLoginCode', 'email', 3),
(1, NULL, 'System:TwoFactorLoginCode', 'sms', 4),
(1, NULL, 'System:AcceptInvitation', 'email', 5)

GO

insert into [AspNetRoles]
(Id, ConcurrencyStamp, Name, NormalizedName, TenantId)
values
('A0AC1B16-A16B-43A1-AD9C-A58C6E93501D', '04734539-33A5-420E-9652-E3C781851913', 'configure', 'configure', 1),
('5967A567-3892-4648-A6E6-A6EEEF0EDD0A', '6D171CE0-1A63-4753-8243-97F170BDCB5D', 'admin', 'admin', 1),
('60F299EF-F722-490E-B41C-4DD64FFBEECD', 'E16406F6-971A-418C-B30E-46F4EE1D6680', 'basic', 'basic', 1)
GO

insert into AspNetRoleClaims
(ClaimType, ClaimValue, RoleId)
values
('urn:traffk.com/claims/ReleaseLog', '{"Granted":true, "Version":1}', 'A0AC1B16-A16B-43A1-AD9C-A58C6E93501D'),
('urn:traffk.com/claims/ManageTenants', '{"Granted":true, "Version":1}', 'A0AC1B16-A16B-43A1-AD9C-A58C6E93501D'),
('urn:traffk.com/claims/ManageRoles', '{"Granted":true, "Version":1}', 'A0AC1B16-A16B-43A1-AD9C-A58C6E93501D'),
('urn:traffk.com/claims/BasicReporting', '{"Granted":true, "Version":1}', 'A0AC1B16-A16B-43A1-AD9C-A58C6E93501D'),
('urn:traffk.com/claims/ManageUsers', '{"Granted":true, "Version":1}', 'A0AC1B16-A16B-43A1-AD9C-A58C6E93501D'),
('urn:traffk.com/claims/ManageTenants', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/ReleaseLog', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/ManageRoles', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/BasicReporting', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/CustomerRelationshipData', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/DirectMessaging', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/ProtectedHealthInformation', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/ManageUsers', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/Messaging', '{"Granted":true, "Version":1}', '5967A567-3892-4648-A6E6-A6EEEF0EDD0A'),
('urn:traffk.com/claims/ReleaseLog', '{"Granted":true, "Version":1}', '60F299EF-F722-490E-B41C-4DD64FFBEECD'),
('urn:traffk.com/claims/BasicReporting', '{"Granted":true, "Version":1}', '60F299EF-F722-490E-B41C-4DD64FFBEECD'),
('urn:traffk.com/claims/CustomerRelationshipData', '{"Granted":true, "Version":1}', '60F299EF-F722-490E-B41C-4DD64FFBEECD');


GO

--As there are no users, this will have to be run manually!
insert into [dbo].[AspNetUserRoles]
(UserId, RoleId)
select distinct u.Id, 'A0AC1B16-A16B-43A1-AD9C-A58C6E93501D'--r.Id
from [dbo].[AspNetUsers] u, [dbo].[AspNetRoles] r

GO
