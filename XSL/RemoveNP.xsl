<?xml version="1.0"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" indent="yes" encoding="utf-8"/>

    <xsl:template match="/">
        <board>
            <parts>
                <xsl:apply-templates select="board/parts"/>
            </parts>
        </board>
    </xsl:template>

    <xsl:template match="part">
        <xsl:if test="attributes/attribute[@name='MPN'] and (not (attributes/attribute[@name='NP']))">
            <part>
                <xsl:copy-of select="@*|node()"/>
            </part>
        </xsl:if>
    </xsl:template>

</xsl:stylesheet>