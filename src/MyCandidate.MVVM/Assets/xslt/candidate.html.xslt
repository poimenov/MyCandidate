<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns="http://www.w3.org/1999/xhtml" xmlns:ext="urn:ExtObj">
  <xsl:output method="html" indent="yes" omit-xml-declaration="yes"/>
  <xsl:variable name="name" select="concat(/Candidate/@lastName, ' ', /Candidate/@firstName)"/>

  <xsl:template match="/">
    <html>
      <head>
        <title><xsl:value-of select="ext:GetLocalized('Candidate')"/>: <xsl:value-of select="$name"/></title>
        <style>
        table {
          width: 100%;
          font-family: Helvetica,Arial,Sans-serif;
          font-size: 12pt;
        }  
        td {
          padding: 4px;
          vertical-align: top;
        }
        fieldset {
          padding: 4px;
          border: 1px solid black;
          border-radius: 4px;		          
        }
        legend {
          font-weight: bold;
        }
        .flex-container {
          display: flex;
          flex-flow: row wrap; 
          height: 63px; 
          overflow-y: auto;          
        }
        div {
          font-family: Helvetica,Arial,Sans-serif;
          font-size: 12pt;
        }
        .lbl {
          font-weight: bold;
        }
        .txt {
          border: 1px solid black;
          border-radius: 4px;
        }   
        .skill {
          border: 1px solid black;
          border-radius: 4px;
          padding: 4px 0px 3px 0px;
          margin: 2px;
          height:18px;
          white-space: nowrap;
        } 
        .resource {
          border: 1px solid black;
          border-radius: 4px;
          padding: 1px;
          margin: 2px;
          height:24px;
          white-space: nowrap;
        }         
        .skill span {
          padding:4px;
	        white-space: nowrap;
        } 
        .resource span {
          height:24px;
          vertical-align: middle;
        }        
        .resource span a {
          color: black;
          margin: 0px 4px 0px 4px;
          white-space: nowrap;
        }
        .black {
          border-right: 1px solid black;
          background-color: gray;
          color: white;
          border-radius: 4px 0 0 4px;          
        }  
        .candidates {
          border-collapse:collapse;
          width:100%;          
        } 
        .candidates th {
          border: 1px solid black;
          padding: 2px;
        }  
        .candidates td {
          border: 1px solid black;
          padding: 2px;
        }                   
        .path {
          width: 24px;
          height: 24px;
          display:inline-block;
          background-repeat: no-repeat;
          background-position: center;
        <xsl:value-of disable-output-escaping="yes" select="ext:GetSvg('fine-print-svgrepo-com.svg',24,24,true())"/>  
        }        
        .mobile {
          width: 24px;
          height: 24px;
          display:inline-block;
          background-repeat: no-repeat;
          background-position: center;
        <xsl:value-of disable-output-escaping="yes" select="ext:GetSvg('mobile-phone-app-svgrepo-com.svg',24,24,true())"/>  
        }
        .email {
          width: 24px;
          height: 24px;
          display:inline-block;
          background-repeat: no-repeat;
          background-position: center;
        <xsl:value-of disable-output-escaping="yes" select="ext:GetSvg('mail-part-2-svgrepo-com.svg',24,24,true())"/>  
        }   
        .skype {
          width: 24px;
          height: 24px;
          display:inline-block;
          background-repeat: no-repeat;
          background-position: center;
        <xsl:value-of disable-output-escaping="yes" select="ext:GetSvg('skype-svgrepo-com.svg',24,24,true())"/>  
        }   
        .url {
          width: 24px;
          height: 24px;
          display:inline-block;
          background-repeat: no-repeat;
          background-position: center;
        <xsl:value-of disable-output-escaping="yes" select="ext:GetSvg('url-internet-svgrepo-com.svg',24,24,true())"/>  
        }                   
        </style>
      </head>
      <xsl:apply-templates select="Candidate"/>
    </html>
  </xsl:template>	
  
    <xsl:template match="Candidate">
    <body>
      <h2 style="text-align: center;"><xsl:value-of select="ext:GetLocalized('Candidate')"/>: <xsl:value-of select="$name"/></h2>
      <table>
        <colgroup>
          <col span="1" style="width: 50%;" />
          <col span="1" style="width: 50%;" />
        </colgroup>
        <tr>
          <td>
            <table>
              <colgroup>
                <col span="1" style="width: 100px;" />
                <col span="1" />
              </colgroup>            
              <tr>
                <td class="lbl"><xsl:value-of select="ext:GetLocalized('Name')"/></td>
                <td class="txt"><xsl:value-of select="$name"/></td>
              </tr> 
              <tr>
                <td class="lbl"><xsl:value-of select="ext:GetLocalized('Title')"/></td>
                <td class="txt"><xsl:value-of select="@title"/></td>
              </tr>               
            </table>         
          </td>
          <td>
            <table>
              <colgroup>
                <col span="1" style="width: 180px;" />
                <col span="1" />
              </colgroup>               
              <tr>
                <td class="lbl"><xsl:value-of select="ext:GetLocalized('Address')"/></td>
                <td class="txt"><xsl:apply-templates select="Location"/></td>
              </tr>                
              <tr>
                <td class="lbl"><xsl:value-of select="ext:GetLocalized('Enabled')"/></td>
                <td class="txt"><xsl:value-of select="@enabled"/></td>
              </tr>                                         
            </table>           
          </td>
        </tr>
        <tr>
          <td>
            <fieldset>
              <legend><xsl:value-of select="ext:GetLocalized('Links')"/></legend>
              <div class="flex-container">
                <xsl:apply-templates select="Resources/Resource"/>
              </div>
            </fieldset>
          </td>
          <td>
            <fieldset>
              <legend><xsl:value-of select="ext:GetLocalized('Skills')"/></legend>
              <div class="flex-container">
                <xsl:apply-templates select="Skills/Skill"/>
              </div>
            </fieldset>          
          </td>
        </tr>        
        <tr>
          <td colspan="2">
            <fieldset>
              <legend><xsl:value-of select="ext:GetLocalized('Vacancies')"/></legend>
              <div style="overflow-x: auto;">
                <table class="candidates">
                <tr style="background-color: Gainsboro;">
                  <th><xsl:value-of select="ext:GetLocalized('Title')"/></th>  
                  <th><xsl:value-of select="ext:GetLocalized('Status')"/></th>                                 
                  <th><xsl:value-of select="ext:GetLocalized('Company')"/></th>
                  <th><xsl:value-of select="ext:GetLocalized('Selection_Status')"/></th>                                    
                  <th><xsl:value-of select="ext:GetLocalized('Links')"/></th>
                  <th><xsl:value-of select="ext:GetLocalized('Skills')"/></th>
                </tr>
                <xsl:apply-templates select="CandidateOnVacancies/CandidateOnVacancy"/>
                </table>
              </div>              
            </fieldset>            
          </td>
        </tr>
      </table>
    </body>
  </xsl:template>

  <xsl:template match="Location">
    <xsl:value-of select="concat(@address, ', ', @city, ', ', @country)"/>
  </xsl:template>

  <xsl:template match="Skill">
    <span class="skill">
      <span class="black"><xsl:value-of select="@name"/></span>
      <span><xsl:value-of select="@seniority"/></span>
    </span>    
  </xsl:template>  

  <xsl:template match="Resource">
    <span class="resource">
      <span>
        <xsl:choose>
          <xsl:when test="@type='Path'"><xsl:attribute name="class">path</xsl:attribute></xsl:when>
          <xsl:when test="@type='Mobile'"><xsl:attribute name="class">mobile</xsl:attribute></xsl:when>
          <xsl:when test="@type='Email'"><xsl:attribute name="class">email</xsl:attribute></xsl:when>
          <xsl:when test="@type='Skype'"><xsl:attribute name="class">skype</xsl:attribute></xsl:when>
          <xsl:when test="@type='Url'"><xsl:attribute name="class">url</xsl:attribute></xsl:when>
        </xsl:choose>
        <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>        
      </span>
      <span>
        <xsl:choose>
          <xsl:when test="@type='Path' or @type='Url'">
            <a target="_blank">
              <xsl:attribute name="href"><xsl:value-of select="@value"/></xsl:attribute>
              <xsl:attribute name="title"><xsl:value-of select="@value"/></xsl:attribute>
              <xsl:value-of select="ext:GetResourceText(@value)"/>
            </a>          
          </xsl:when>
          <xsl:when test="@type='Email'">
            <a>
              <xsl:attribute name="href">mailto:<xsl:value-of select="@value"/></xsl:attribute>
              <xsl:value-of select="@value"/>
            </a>
          </xsl:when>
          <xsl:otherwise><xsl:value-of select="@value"/></xsl:otherwise>
        </xsl:choose>            
      </span>
    </span>    
  </xsl:template>   
  
<xsl:template match="CandidateOnVacancy">
  <tr>
    <td><xsl:value-of select="Vacancy/@title"/></td>
    <td><xsl:value-of select="Vacancy/VacancyStatus/@name"/></td>
    <td><xsl:value-of select="concat(Vacancy/Company/@name, ', ',Vacancy/Company/Office/@name)"/></td>
    <td><xsl:value-of select="@status"/></td>
    <td><div class="flex-container"><xsl:apply-templates select="Vacancy/Resources/Resource"/></div></td>
    <td><div class="flex-container"><xsl:apply-templates select="Vacancy/Skills/Skill"/></div></td>
  </tr>
</xsl:template>
  
</xsl:stylesheet>    
    
    