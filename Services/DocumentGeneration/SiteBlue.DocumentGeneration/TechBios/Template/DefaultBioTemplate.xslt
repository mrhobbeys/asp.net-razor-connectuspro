<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN" indent="yes" standalone="yes"  />
  <xsl:decimal-format name="usd" decimal-separator="." grouping-separator=","/>
  <xsl:decimal-format name="qty" decimal-separator="." grouping-separator=","/>
  <xsl:template match="Bio">
    <html>
      <head>
        <title>
          Technician Bio - <xsl:value-of select="Name"/>
        </title>
        <style type="text/css" media="print, screen">
          body
          {
          font-family: Tahoma;
          font-size: 14px;
          margin:0;
          padding:0;
          }
          h2
          {
          font-weight: bold;
          margin: 0;
          font-size: 170%;
          }
          h3
          {
          font-weight: bold;
          margin: 0;
          font-size: 150%;
          }
          h4
          {
          font-weight: bold;
          margin: 0;
          font-size: 130%;
          }
          .right
          {
          float: right;
          text-align: right;
          }
          .left
          {
          clear: both;
          float: left;
          margin-bottom: 20px;
          }
          .header
          {
          clear: left;
          float: left;
          width: 100%;
          }
          .fromto
          {
          clear: left;
          float: left;
          width: 100%;
          }
          .heading
          {
          font-weight:bold;
          }
          .pairs
          {
          width:275px;
          }
          .pairs span
          {
          clear:left;
          float:left;
          width:200px;
          }
          .bioText
          {
          clear:both;
          }
          
        </style>
      </head>
      <body>
        <div id="invoiceContainer">
          <div class="header">
            <div class="left">
              <h3>
                Technician Bio
              </h3>
              <h2>
                <xsl:value-of select="Name"/>
              </h2>
            </div>
            <div class="right">
              <img>
                <xsl:attribute name="src">
                  http://conn951.gearhost.us.com/fileutils/franchiseimages/<xsl:value-of select="ClientId"/>_dbaimage.jpg
                </xsl:attribute>
              </img>
            </div>
          </div>
          <div class="fromto bottomSep">
            <div>
              <div class="left">
                <img style="width:200px">
                  <xsl:attribute name="src">
                    http://conn951.gearhost.us.com/fileutils/techimages/<xsl:value-of select="TechId"/>.png
                  </xsl:attribute>
                </img>
              </div>
              <div class="right pairs">
                <div>
                  <span>Last Drug Test:</span>
                  <xsl:call-template name="formatDate">
                    <xsl:with-param name="dateTime" select="LastDrugTest"/>
                  </xsl:call-template>
                </div>
                <div>
                  <span>Background Check Complete:</span>
                  <xsl:call-template name="formatDate">
                    <xsl:with-param name="dateTime" select="BackgroundCheckCompleted"/>
                  </xsl:call-template>
                </div>
              </div>
            </div>
            <div class="bioText">
              <xsl:value-of select="Text"/>
            </div>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
  <xsl:template name="formatDate">
    <xsl:param name="dateTime" />
    <xsl:variable name="date" select="substring-before($dateTime, 'T')" />
    <xsl:variable name="year" select="substring-before($date, '-')" />
    <xsl:variable name="month" select="substring-before(substring-after($date, '-'), '-')" />
    <xsl:variable name="day" select="substring-after(substring-after($date, '-'), '-')" />
    <xsl:value-of select="concat($month, '/', $day, '/', $year)" />
  </xsl:template>

  <xsl:template name="formatTime">
    <xsl:param name="dateTime" />
    <xsl:value-of select="substring-after($dateTime, 'T')" />
  </xsl:template>

</xsl:stylesheet>
